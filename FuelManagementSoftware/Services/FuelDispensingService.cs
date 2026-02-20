using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service for orchestrating automated fuel dispensing operations.
/// Manages the complete workflow from card authentication to transaction completion.
/// </summary>
public class FuelDispensingService : IFuelDispensingService
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly INfcReaderService _nfcReaderService;
    private readonly IPetroCardService _petroCardService;
    private readonly IFuelStockService _fuelStockService;
    private readonly IPumpControlService _pumpControlService;
    private readonly IBlockchainService _blockchainService;
    private readonly ILogger<FuelDispensingService> _logger;

    public FuelDispensingService(
        FilteredFuelManagementSoftwareDbContext context,
        INfcReaderService nfcReaderService,
        IPetroCardService petroCardService,
        IFuelStockService fuelStockService,
        IPumpControlService pumpControlService,
        IBlockchainService blockchainService,
        ILogger<FuelDispensingService> logger)
    {
        _context = context;
        _nfcReaderService = nfcReaderService;
        _petroCardService = petroCardService;
        _fuelStockService = fuelStockService;
        _pumpControlService = pumpControlService;
        _blockchainService = blockchainService;
        _logger = logger;
    }

    public async Task<FuelTransaction> InitiateDispensingAsync(DispensingRequest request, CancellationToken cancellationToken = default)
    {
        var isCash = string.Equals(request.PaymentMethod, "Cash", StringComparison.OrdinalIgnoreCase);
        _logger.LogInformation("Initiating fuel dispensing: Station {StationId}, Pump {PumpId}, FuelType {FuelTypeId}, Payment: {PaymentMethod}",
            request.FuelStationId, request.FuelPumpId, request.FuelTypeId, request.PaymentMethod ?? "PetroCard");

        PetroCard? card = null;
        if (!isCash)
        {
            if (string.IsNullOrWhiteSpace(request.NfcTag))
                throw new InvalidOperationException("Card ID is required for PetroCard payment.");
            if (!_nfcReaderService.ValidateNfcTag(request.NfcTag!))
                throw new InvalidOperationException("Invalid NFC tag format");
            card = await _petroCardService.GetCardByNfcTagAsync(request.NfcTag!, cancellationToken);
            if (card == null)
                throw new InvalidOperationException("PetroCard not found for the provided NFC tag");
            if (!string.IsNullOrWhiteSpace(request.Pin) && !_petroCardService.VerifyPin(card, request.Pin))
                throw new UnauthorizedAccessException("Invalid PIN");
        }

        var pump = await _context.FuelPumps
            .Include(p => p.FuelStation)
            .Include(p => p.FuelType)
            .FirstOrDefaultAsync(p => p.Id == request.FuelPumpId && p.FuelStationId == request.FuelStationId, cancellationToken);

        if (pump == null)
            throw new InvalidOperationException($"Fuel pump {request.FuelPumpId} not found at station {request.FuelStationId}");
        if (!pump.IsOperational || !pump.IsActive)
            throw new InvalidOperationException($"Fuel pump {request.FuelPumpId} is not operational");
        if (!pump.FuelStation.IsOpen || !pump.FuelStation.IsActive)
            throw new InvalidOperationException($"Fuel station {request.FuelStationId} is not open");
        if (pump.FuelStation.IsTankerOffloading)
            throw new InvalidOperationException($"Fuel station {request.FuelStationId} is currently offloading and closed");

        var fuelType = await _context.FuelTypes
            .FirstOrDefaultAsync(ft => ft.Id == request.FuelTypeId, cancellationToken);
        if (fuelType == null || !fuelType.IsActive)
            throw new InvalidOperationException($"Fuel type {request.FuelTypeId} not found or inactive");

        decimal maxQuantity;
        int organisationId;
        string currency;

        if (isCash)
        {
            maxQuantity = await _fuelStockService.GetMaxDispenseableQuantityAsync(request.FuelStationId, request.FuelTypeId, cancellationToken);
            organisationId = pump.FuelStation.OrganisationId;
            currency = "USD";
        }
        else
        {
            var maxByBalance = card!.Balance / fuelType.UnitPrice;
            var maxByStock = await _fuelStockService.GetMaxDispenseableQuantityAsync(request.FuelStationId, request.FuelTypeId, cancellationToken);
            maxQuantity = Math.Min(maxByBalance, maxByStock);
            organisationId = card.OrganisationId;
            currency = card.Currency;
        }

        if (maxQuantity <= 0)
            throw new InvalidOperationException(isCash ? "Insufficient fuel stock for dispensing" : "Insufficient balance or stock for dispensing");

        decimal finalQuantity = request.RequestedQuantity ?? maxQuantity;
        if (request.RequestedQuantity.HasValue && request.RequestedQuantity.Value > maxQuantity)
        {
            finalQuantity = maxQuantity;
            _logger.LogWarning("Requested quantity {Requested} exceeds maximum {Max}, using maximum", request.RequestedQuantity, maxQuantity);
        }

        var unitPrice = fuelType.UnitPrice;
        var totalAmount = finalQuantity * unitPrice;

        if (!isCash)
        {
            var cardValidation = await _petroCardService.ValidateCardAsync(card!, totalAmount, cancellationToken);
            if (!cardValidation.IsValid)
                throw new InvalidOperationException(cardValidation.ErrorMessage ?? "Card validation failed");
        }

        var hasStock = await _fuelStockService.HasSufficientStockAsync(request.FuelStationId, request.FuelTypeId, finalQuantity, cancellationToken);
        if (!hasStock)
            throw new InvalidOperationException("Insufficient fuel stock available");

        var transactionNumber = GenerateTransactionNumber();
        var transaction = new FuelTransaction
        {
            OrganisationId = organisationId,
            TransactionNumber = transactionNumber,
            FuelStationId = request.FuelStationId,
            FuelPumpId = request.FuelPumpId,
            FuelTypeId = request.FuelTypeId,
            PetroCardId = isCash ? null : card!.Id,
            UserId = isCash ? null : card!.UserId,
            Quantity = finalQuantity,
            UnitPrice = unitPrice,
            TotalAmount = totalAmount,
            Currency = currency,
            PaymentMethod = isCash ? "Cash" : "PetroCard",
            TransactionStatus = "Authorized",
            TransactionDate = DateTime.UtcNow,
            StartedAt = null,
            CompletedAt = null,
            CreatorId = request.CreatorId
        };

        _context.FuelTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        if (!isCash)
            await _petroCardService.UpdateLastUsedAsync(card!.Id, cancellationToken);

        _logger.LogInformation("Fuel dispensing transaction {TransactionNumber} authorized. Quantity: {Quantity}, Amount: {Amount}, Payment: {PaymentMethod}",
            transactionNumber, finalQuantity, totalAmount, transaction.PaymentMethod);

        return transaction;
    }

    public async Task<FuelTransaction> StartDispensingAsync(int transactionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting fuel dispensing for transaction {TransactionId}", transactionId);

        var transaction = await _context.FuelTransactions
            .Include(t => t.FuelPump)
            .Include(t => t.PetroCard)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction {transactionId} not found");
        }

        if (transaction.TransactionStatus != "Authorized" && transaction.TransactionStatus != "Pending")
        {
            throw new InvalidOperationException($"Transaction {transactionId} is in {transaction.TransactionStatus} status and cannot be started");
        }

        // Start the pump hardware
        var pumpStarted = await _pumpControlService.StartPumpAsync(transaction.FuelPumpId, cancellationToken);
        
        if (!pumpStarted)
        {
            throw new InvalidOperationException($"Failed to start pump {transaction.FuelPumpId}");
        }

        transaction.TransactionStatus = "Dispensing";
        transaction.StartedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fuel dispensing started for transaction {TransactionId}", transactionId);

        return transaction;
    }

    public async Task<FuelTransaction> UpdateDispensingProgressAsync(int transactionId, decimal quantityDispensed, CancellationToken cancellationToken = default)
    {
        var transaction = await _context.FuelTransactions
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction {transactionId} not found");
        }

        if (transaction.TransactionStatus != "Dispensing")
        {
            throw new InvalidOperationException($"Transaction {transactionId} is not in Dispensing status");
        }

        // Update quantity and recalculate amount
        transaction.Quantity = quantityDispensed;
        transaction.TotalAmount = quantityDispensed * transaction.UnitPrice;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Updated dispensing progress for transaction {TransactionId}: {Quantity} litres", 
            transactionId, quantityDispensed);

        return transaction;
    }

    public async Task<FuelTransaction> CompleteDispensingAsync(int transactionId, decimal finalQuantity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing fuel dispensing for transaction {TransactionId} with quantity {Quantity}", 
            transactionId, finalQuantity);

        var transaction = await _context.FuelTransactions
            .Include(t => t.PetroCard)
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction {transactionId} not found");
        }

        if (transaction.TransactionStatus != "Dispensing")
        {
            throw new InvalidOperationException($"Transaction {transactionId} is not in Dispensing status");
        }

        // Update final quantity and amount
        transaction.Quantity = finalQuantity;
        transaction.TotalAmount = finalQuantity * transaction.UnitPrice;
        transaction.TransactionStatus = "Completed";
        transaction.CompletedAt = DateTime.UtcNow;

        // Stop the pump hardware
        await _pumpControlService.StopPumpAsync(transaction.FuelPumpId, cancellationToken);

        // Deduct from card balance only when paid by PetroCard
        if (transaction.PetroCardId.HasValue)
        {
            await _petroCardService.DeductBalanceAsync(
                transaction.PetroCardId.Value,
                transaction.TotalAmount,
                "FuelPurchase",
                transaction.TransactionNumber,
                transaction.CreatorId,
                cancellationToken);
        }

        // Deduct from stock
        await _fuelStockService.DeductStockAsync(
            transaction.FuelStationId,
            transaction.FuelTypeId,
            finalQuantity,
            transaction.CreatorId,
            transaction.TransactionNumber,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        // Record to blockchain
        try
        {
            if (_blockchainService.IsConfigured())
            {
                // Ensure PetroCard is loaded for hashing
                if (transaction.PetroCard == null && transaction.PetroCardId.HasValue)
                {
                    await _context.Entry(transaction).Reference(t => t.PetroCard).LoadAsync(cancellationToken);
                }

                var blockchainResult = await _blockchainService.RecordTransactionAsync(transaction, cancellationToken);

                if (blockchainResult.Success)
                {
                    var previousHash = await _context.BlockchainTransactions
                        .Where(bt => bt.OrganisationId == transaction.OrganisationId && bt.Status == "Confirmed")
                        .OrderByDescending(bt => bt.CreatedAt)
                        .Select(bt => bt.BlockchainHash)
                        .FirstOrDefaultAsync(cancellationToken);

                    var blockchainTx = new BlockchainTransaction
                    {
                        OrganisationId = transaction.OrganisationId,
                        FuelTransactionId = transaction.Id,
                        BlockchainHash = blockchainResult.TransactionHash!,
                        PreviousHash = previousHash,
                        BlockNumber = blockchainResult.BlockNumber,
                        TransactionIndex = blockchainResult.TransactionIndex,
                        BlockchainNetwork = "Sepolia",
                        SmartContractAddress = blockchainResult.ContractAddress,
                        GasUsed = blockchainResult.GasUsed,
                        Status = "Confirmed",
                        ConfirmationCount = 1,
                        CreatedAt = DateTime.UtcNow,
                        ConfirmedAt = DateTime.UtcNow,
                        CreatorId = transaction.CreatorId
                    };

                    _context.BlockchainTransactions.Add(blockchainTx);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Transaction {TxNumber} recorded on Sepolia blockchain. Hash: {Hash}",
                        transaction.TransactionNumber, blockchainResult.TransactionHash);
                }
                else
                {
                    _logger.LogWarning("Blockchain recording failed for {TxNumber}: {Error}",
                        transaction.TransactionNumber, blockchainResult.ErrorMessage);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blockchain recording failed for transaction {TxNumber}. Fuel transaction still completed.",
                transaction.TransactionNumber);
        }

        _logger.LogInformation("Fuel dispensing completed for transaction {TransactionNumber}. Quantity: {Quantity}, Amount: {Amount}", 
            transaction.TransactionNumber, finalQuantity, transaction.TotalAmount);

        return transaction;
    }

    public async Task<FuelTransaction> CancelDispensingAsync(int transactionId, string? reason = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling fuel dispensing for transaction {TransactionId}. Reason: {Reason}", 
            transactionId, reason ?? "No reason provided");

        var transaction = await _context.FuelTransactions
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction {transactionId} not found");
        }

        if (transaction.TransactionStatus == "Completed")
        {
            throw new InvalidOperationException($"Cannot cancel completed transaction {transactionId}");
        }

        // Stop the pump hardware if dispensing
        if (transaction.TransactionStatus == "Dispensing")
        {
            await _pumpControlService.StopPumpAsync(transaction.FuelPumpId, cancellationToken);
        }

        transaction.TransactionStatus = "Cancelled";
        transaction.Notes = reason ?? "Transaction cancelled";
        transaction.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fuel dispensing cancelled for transaction {TransactionId}", transactionId);

        return transaction;
    }

    private string GenerateTransactionNumber()
    {
        // Generate unique transaction number: TXN-YYYYMMDD-HHMMSS-XXXX
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var random = new Random().Next(1000, 9999);
        return $"TXN-{timestamp}-{random}";
    }
}

