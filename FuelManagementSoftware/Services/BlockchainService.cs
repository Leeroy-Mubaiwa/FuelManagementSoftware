using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace FuelManagementSoftware.Services;

// Nethereum contract deployment message
public class FuelTransactionLedgerDeployment : ContractDeploymentMessage
{
    public static string BYTECODE = "";
    public FuelTransactionLedgerDeployment() : base(BYTECODE) { }
}

// Nethereum typed function: recordTransaction
[Function("recordTransaction", "uint256")]
public class RecordTransactionFunction : FunctionMessage
{
    [Parameter("string", "_txNumber", 1)]
    public string TxNumber { get; set; } = "";
    [Parameter("uint256", "_stationId", 2)]
    public BigInteger StationId { get; set; }
    [Parameter("uint256", "_pumpId", 3)]
    public BigInteger PumpId { get; set; }
    [Parameter("uint256", "_quantity", 4)]
    public BigInteger Quantity { get; set; }
    [Parameter("uint256", "_amount", 5)]
    public BigInteger Amount { get; set; }
    [Parameter("string", "_cardHash", 6)]
    public string CardHash { get; set; } = "";
}

// Nethereum typed function: verifyTransaction
[Function("verifyTransaction", typeof(VerifyTransactionOutput))]
public class VerifyTransactionFunction : FunctionMessage
{
    [Parameter("string", "_txNumber", 1)]
    public string TxNumber { get; set; } = "";
}

[FunctionOutput]
public class VerifyTransactionOutput : IFunctionOutputDTO
{
    [Parameter("bool", "exists", 1)]
    public bool Exists { get; set; }
    [Parameter("uint256", "index", 2)]
    public BigInteger Index { get; set; }
}

// Nethereum typed function: getRecord
[Function("getRecord", typeof(GetRecordOutput))]
public class GetRecordFunction : FunctionMessage
{
    [Parameter("uint256", "_index", 1)]
    public BigInteger Index { get; set; }
}

[FunctionOutput]
public class GetRecordOutput : IFunctionOutputDTO
{
    [Parameter("string", "transactionNumber", 1)]
    public string TransactionNumber { get; set; } = "";
    [Parameter("uint256", "stationId", 2)]
    public BigInteger StationId { get; set; }
    [Parameter("uint256", "pumpId", 3)]
    public BigInteger PumpId { get; set; }
    [Parameter("uint256", "quantity", 4)]
    public BigInteger Quantity { get; set; }
    [Parameter("uint256", "amount", 5)]
    public BigInteger Amount { get; set; }
    [Parameter("string", "cardNumberHash", 6)]
    public string CardNumberHash { get; set; } = "";
    [Parameter("uint256", "timestamp", 7)]
    public BigInteger Timestamp { get; set; }
    [Parameter("address", "recorder", 8)]
    public string Recorder { get; set; } = "";
}

// Nethereum typed function: getRecordCount
[Function("getRecordCount", "uint256")]
public class GetRecordCountFunction : FunctionMessage { }

public class BlockchainService : IBlockchainService
{
    private readonly BlockchainSettings _settings;
    private readonly ILogger<BlockchainService> _logger;
    private readonly IWebHostEnvironment _env;
    private string _contractAddress;

    public BlockchainService(
        IOptions<BlockchainSettings> settings,
        ILogger<BlockchainService> logger,
        IWebHostEnvironment env)
    {
        _settings = settings.Value;
        _logger = logger;
        _env = env;
        _contractAddress = _settings.ContractAddress;

        // Try loading from file if not in config
        if (string.IsNullOrWhiteSpace(_contractAddress))
        {
            var addressFile = Path.Combine(_env.ContentRootPath, "Contracts", "deployed-address.txt");
            if (File.Exists(addressFile))
            {
                _contractAddress = File.ReadAllText(addressFile).Trim();
                _logger.LogInformation("Loaded contract address from file: {Address}", _contractAddress);
            }
        }
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_settings.RpcUrl)
            && !string.IsNullOrWhiteSpace(_settings.PrivateKey)
            && !string.IsNullOrWhiteSpace(_settings.WalletAddress);
    }

    public string GetContractAddress() => _contractAddress;

    public string GetExplorerTransactionUrl(string txHash)
        => $"{_settings.ExplorerUrl}/tx/{txHash}";

    public string GetExplorerAddressUrl(string address)
        => $"{_settings.ExplorerUrl}/address/{address}";

    public async Task<string> EnsureContractDeployedAsync(CancellationToken ct = default)
    {
        if (!IsConfigured())
        {
            _logger.LogWarning("Blockchain not configured. Skipping contract deployment.");
            return "";
        }

        if (!string.IsNullOrWhiteSpace(_contractAddress))
        {
            _logger.LogInformation("Contract already deployed at: {Address}", _contractAddress);
            return _contractAddress;
        }

        _logger.LogInformation("Deploying FuelTransactionLedger contract to {Network}...", _settings.NetworkName);

        var bytecodeFile = Path.Combine(_env.ContentRootPath, "Contracts", "FuelTransactionLedger.bin");
        if (!File.Exists(bytecodeFile))
        {
            throw new FileNotFoundException("Contract bytecode not found. Run the Solidity compiler first.", bytecodeFile);
        }

        FuelTransactionLedgerDeployment.BYTECODE = "0x" + File.ReadAllText(bytecodeFile).Trim();

        var account = new Account(_settings.PrivateKey, _settings.ChainId);
        var web3 = new Web3(account, _settings.RpcUrl);

        var deployment = new FuelTransactionLedgerDeployment();
        deployment.Gas = new Nethereum.Hex.HexTypes.HexBigInteger(3_000_000);

        var handler = web3.Eth.GetContractDeploymentHandler<FuelTransactionLedgerDeployment>();
        var receipt = await handler.SendRequestAndWaitForReceiptAsync(deployment, ct);

        _contractAddress = receipt.ContractAddress;
        _logger.LogInformation("Contract deployed at: {Address} (tx: {TxHash}, block: {Block})",
            _contractAddress, receipt.TransactionHash, receipt.BlockNumber.Value);

        // Persist the address
        var addressFile = Path.Combine(_env.ContentRootPath, "Contracts", "deployed-address.txt");
        await File.WriteAllTextAsync(addressFile, _contractAddress, ct);

        return _contractAddress;
    }

    public async Task<BlockchainRecordResult> RecordTransactionAsync(FuelTransaction transaction, CancellationToken ct = default)
    {
        if (!IsConfigured() || string.IsNullOrWhiteSpace(_contractAddress))
        {
            return new BlockchainRecordResult { Success = false, ErrorMessage = "Blockchain not configured or contract not deployed" };
        }

        try
        {
            var account = new Account(_settings.PrivateKey, _settings.ChainId);
            var web3 = new Web3(account, _settings.RpcUrl);
            var contractHandler = web3.Eth.GetContractHandler(_contractAddress);

            var cardHash = transaction.PetroCard != null
                ? HashString(transaction.PetroCard.CardNumber)
                : "N/A";

            var recordFunction = new RecordTransactionFunction
            {
                TxNumber = transaction.TransactionNumber,
                StationId = new BigInteger(transaction.FuelStationId),
                PumpId = new BigInteger(transaction.FuelPumpId),
                Quantity = new BigInteger((long)(transaction.Quantity * 100)),
                Amount = new BigInteger((long)(transaction.TotalAmount * 100)),
                CardHash = cardHash
            };

            var receipt = await contractHandler.SendRequestAndWaitForReceiptAsync(recordFunction, ct);

            _logger.LogInformation("Transaction {TxNumber} recorded on blockchain. Hash: {Hash}, Block: {Block}",
                transaction.TransactionNumber, receipt.TransactionHash, receipt.BlockNumber.Value);

            return new BlockchainRecordResult
            {
                Success = receipt.Status?.Value == 1,
                TransactionHash = receipt.TransactionHash,
                BlockNumber = (long)receipt.BlockNumber.Value,
                TransactionIndex = (int)receipt.TransactionIndex.Value,
                ContractAddress = _contractAddress,
                GasUsed = (decimal)receipt.GasUsed.Value,
                ErrorMessage = receipt.Status?.Value != 1 ? "Transaction reverted on chain" : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record transaction {TxNumber} on blockchain", transaction.TransactionNumber);
            return new BlockchainRecordResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<BlockchainVerifyResult> VerifyTransactionOnChainAsync(string transactionNumber, CancellationToken ct = default)
    {
        if (!IsConfigured() || string.IsNullOrWhiteSpace(_contractAddress))
        {
            return new BlockchainVerifyResult { Success = false, ErrorMessage = "Blockchain not configured" };
        }

        try
        {
            var account = new Account(_settings.PrivateKey, _settings.ChainId);
            var web3 = new Web3(account, _settings.RpcUrl);
            var contractHandler = web3.Eth.GetContractHandler(_contractAddress);

            // Verify existence
            var verifyResult = await contractHandler.QueryDeserializingToObjectAsync<VerifyTransactionFunction, VerifyTransactionOutput>(
                new VerifyTransactionFunction { TxNumber = transactionNumber }, null);

            if (!verifyResult.Exists)
            {
                return new BlockchainVerifyResult
                {
                    Success = true,
                    ExistsOnChain = false,
                    TransactionNumber = transactionNumber
                };
            }

            // Get full record
            var record = await contractHandler.QueryDeserializingToObjectAsync<GetRecordFunction, GetRecordOutput>(
                new GetRecordFunction { Index = verifyResult.Index }, null);

            var timestamp = DateTimeOffset.FromUnixTimeSeconds((long)record.Timestamp).UtcDateTime;

            return new BlockchainVerifyResult
            {
                Success = true,
                ExistsOnChain = true,
                RecordIndex = (long)verifyResult.Index,
                TransactionNumber = record.TransactionNumber,
                StationId = (long)record.StationId,
                PumpId = (long)record.PumpId,
                Quantity = (decimal)record.Quantity / 100m,
                Amount = (decimal)record.Amount / 100m,
                CardNumberHash = record.CardNumberHash,
                BlockchainTimestamp = timestamp,
                RecorderAddress = record.Recorder
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify transaction {TxNumber} on blockchain", transactionNumber);
            return new BlockchainVerifyResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<long> GetRecordCountAsync(CancellationToken ct = default)
    {
        if (!IsConfigured() || string.IsNullOrWhiteSpace(_contractAddress))
            return 0;

        try
        {
            var web3 = new Web3(_settings.RpcUrl);
            var contractHandler = web3.Eth.GetContractHandler(_contractAddress);
            var result = await contractHandler.QueryAsync<GetRecordCountFunction, BigInteger>(null, null);
            return (long)result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get record count from blockchain");
            return 0;
        }
    }

    private static string HashString(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

