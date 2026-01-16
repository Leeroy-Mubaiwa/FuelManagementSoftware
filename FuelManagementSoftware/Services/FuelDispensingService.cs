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
    private readonly ILogger<FuelDispensingService> _logger;

    public FuelDispensingService(
        FilteredFuelManagementSoftwareDbContext context,
        INfcReaderService nfcReaderService,
        IPetroCardService petroCardService,
        IFuelStockService fuelStockService,
        IPumpControlService pumpControlService,
        ILogger<FuelDispensingService> logger)
    {
        _context = context;
        _nfcReaderService = nfcReaderService;
        _petroCardService = petroCardService;
        _fuelStockService = fuelStockService;
        _pumpControlService = pumpControlService;
        _logger = logger;
    }

    public async Task<FuelTransaction> InitiateDispensingAsync(DispensingRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initiating fuel dispensing: Station {StationId}, Pump {PumpId}, FuelType {FuelTypeId}", 
            request.FuelStationId, request.FuelPumpId, request.FuelTypeId);

        // Validate NFC tag format
        if (!_nfcReaderService.ValidateNfcTag(request.NfcTag))
        {
            throw new InvalidOperationException("Invalid NFC tag format");
        }

        // Get and validate card
        var card = await _petroCardService.GetCardByNfcTagAsync(request.NfcTag, cancellationToken);
        if (card == null)
        {
            throw new InvalidOperationException("PetroCard not found for the provided NFC tag");
        }

        // Verify PIN if provided
        if (!string.IsNullOrWhiteSpace(request.Pin))
        {
            if (!_petroCardService.VerifyPin(card, request.Pin))
            {
                throw new UnauthorizedAccessException("Invalid PIN");
            }
        }

        // Get pump and station information
        var pump = await _context.FuelPumps
            .Include(p => p.FuelStation)
            .Include(p => p.FuelType)
            .FirstOrDefaultAsync(p => p.Id == request.FuelPumpId && p.FuelStationId == request.FuelStationId, cancellationToken);

        if (pump == null)
        {
            throw new InvalidOperationException($"Fuel pump {request.FuelPumpId} not found at station {request.FuelStationId}");
        }

        // Validate pump is operational
        if (!pump.IsOperational || !pump.IsActive)
        {
            throw new InvalidOperationException($"Fuel pump {request.FuelPumpId} is not operational");
        }

        // Validate station is open
        if (!pump.FuelStation.IsOpen || !pump.FuelStation.IsActive)
        {
            throw new InvalidOperationException($"Fuel station {request.FuelStationId} is not open");
        }

        // Check if station is offloading
        if (pump.FuelStation.IsTankerOffloading)
        {
            throw new InvalidOperationException($"Fuel station {request.FuelStationId} is currently offloading and closed");
        }

        // Get fuel type and pricing
        var fuelType = await _context.FuelTypes
            .FirstOrDefaultAsync(ft => ft.Id == request.FuelTypeId, cancellationToken);

        if (fuelType == null || !fuelType.IsActive)
        {
            throw new InvalidOperationException($"Fuel type {request.FuelTypeId} not found or inactive");
        }

        // Calculate maximum dispenseable quantity based on balance and stock
        var maxByBalance = card.Balance / fuelType.UnitPrice;
        var maxByStock = await _fuelStockService.GetMaxDispenseableQuantityAsync(request.FuelStationId, request.FuelTypeId, cancellationToken);
        var maxQuantity = Math.Min(maxByBalance, maxByStock);

        if (maxQuantity <= 0)
        {
            throw new InvalidOperationException("Insufficient balance or stock for dispensing");
        }

        // If requested quantity is specified, validate it doesn't exceed maximum
        decimal finalQuantity = request.RequestedQuantity ?? maxQuantity;
        if (request.RequestedQuantity.HasValue && request.RequestedQuantity.Value > maxQuantity)
        {
            finalQuantity = maxQuantity;
            _logger.LogWarning("Requested quantity {Requested} exceeds maximum {Max}, using maximum", 
                request.RequestedQuantity, maxQuantity);
        }

        // Calculate amounts
        var unitPrice = fuelType.UnitPrice;
        var totalAmount = finalQuantity * unitPrice;

        // Validate card has sufficient balance
        var cardValidation = await _petroCardService.ValidateCardAsync(card, totalAmount, cancellationToken);
        if (!cardValidation.IsValid)
        {
            throw new InvalidOperationException(cardValidation.ErrorMessage ?? "Card validation failed");
        }

        // Check stock availability
        var hasStock = await _fuelStockService.HasSufficientStockAsync(request.FuelStationId, request.FuelTypeId, finalQuantity, cancellationToken);
        if (!hasStock)
        {
            throw new InvalidOperationException("Insufficient fuel stock available");
        }

        // Generate unique transaction number
        var transactionNumber = GenerateTransactionNumber();

        // Create fuel transaction
        var transaction = new FuelTransaction
        {
            OrganisationId = card.OrganisationId,
            TransactionNumber = transactionNumber,
            FuelStationId = request.FuelStationId,
            FuelPumpId = request.FuelPumpId,
            FuelTypeId = request.FuelTypeId,
            PetroCardId = card.Id,
            UserId = card.UserId,
            Quantity = finalQuantity,
            UnitPrice = unitPrice,
            TotalAmount = totalAmount,
            Currency = card.Currency,
            PaymentMethod = "PetroCard",
            TransactionStatus = "Authorized",
            TransactionDate = DateTime.UtcNow,
            StartedAt = null,
            CompletedAt = null,
            CreatorId = request.CreatorId
        };

        _context.FuelTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        // Update card last used
        await _petroCardService.UpdateLastUsedAsync(card.Id, cancellationToken);

        _logger.LogInformation("Fuel dispensing transaction {TransactionNumber} authorized. Quantity: {Quantity}, Amount: {Amount}", 
            transactionNumber, finalQuantity, totalAmount);

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

        // Deduct from card balance
        await _petroCardService.DeductBalanceAsync(
            transaction.PetroCardId!.Value,
            transaction.TotalAmount,
            "FuelPurchase",
            transaction.TransactionNumber,
            transaction.CreatorId,
            cancellationToken);

        // Deduct from stock
        await _fuelStockService.DeductStockAsync(
            transaction.FuelStationId,
            transaction.FuelTypeId,
            finalQuantity,
            transaction.CreatorId,
            transaction.TransactionNumber,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

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

