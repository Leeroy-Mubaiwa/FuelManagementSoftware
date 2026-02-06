using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Models;

namespace FuelManagementSoftware.Services;

public interface IBlockchainService
{
    Task<string> EnsureContractDeployedAsync(CancellationToken ct = default);
    Task<BlockchainRecordResult> RecordTransactionAsync(FuelTransaction transaction, CancellationToken ct = default);
    Task<BlockchainVerifyResult> VerifyTransactionOnChainAsync(string transactionNumber, CancellationToken ct = default);
    Task<long> GetRecordCountAsync(CancellationToken ct = default);
    string GetContractAddress();
    string GetExplorerTransactionUrl(string txHash);
    string GetExplorerAddressUrl(string address);
    bool IsConfigured();
}

public class BlockchainRecordResult
{
    public bool Success { get; set; }
    public string? TransactionHash { get; set; }
    public long BlockNumber { get; set; }
    public int TransactionIndex { get; set; }
    public string? ContractAddress { get; set; }
    public decimal GasUsed { get; set; }
    public string? ErrorMessage { get; set; }
}

public class BlockchainVerifyResult
{
    public bool Success { get; set; }
    public bool ExistsOnChain { get; set; }
    public long RecordIndex { get; set; }
    public string? TransactionNumber { get; set; }
    public long StationId { get; set; }
    public long PumpId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public string? CardNumberHash { get; set; }
    public DateTime? BlockchainTimestamp { get; set; }
    public string? RecorderAddress { get; set; }
    public string? ErrorMessage { get; set; }
}

