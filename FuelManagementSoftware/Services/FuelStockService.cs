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
/// Service for managing fuel stock operations.
/// </summary>
public class FuelStockService : IFuelStockService
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly ILogger<FuelStockService> _logger;

    public FuelStockService(FilteredFuelManagementSoftwareDbContext context, ILogger<FuelStockService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FuelStock?> GetStockAsync(int stationId, int fuelTypeId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting fuel stock for station {StationId}, fuel type {FuelTypeId}", stationId, fuelTypeId);

        var stock = await _context.FuelStocks
            .Include(s => s.FuelStation)
            .Include(s => s.FuelType)
            .FirstOrDefaultAsync(s => s.FuelStationId == stationId && s.FuelTypeId == fuelTypeId, cancellationToken);

        return stock;
    }

    public async Task<bool> HasSufficientStockAsync(int stationId, int fuelTypeId, decimal requiredQuantity, CancellationToken cancellationToken = default)
    {
        var stock = await GetStockAsync(stationId, fuelTypeId, cancellationToken);

        if (stock == null)
        {
            _logger.LogWarning("No stock record found for station {StationId}, fuel type {FuelTypeId}", stationId, fuelTypeId);
            return false;
        }

        var hasStock = stock.CurrentQuantity >= requiredQuantity;

        if (!hasStock)
        {
            _logger.LogWarning("Insufficient stock for station {StationId}, fuel type {FuelTypeId}. Required: {Required}, Available: {Available}", 
                stationId, fuelTypeId, requiredQuantity, stock.CurrentQuantity);
        }

        return hasStock;
    }

    public async Task<FuelStock> DeductStockAsync(int stationId, int fuelTypeId, decimal quantity, string creatorId, string? referenceNumber = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deducting {Quantity} litres from stock for station {StationId}, fuel type {FuelTypeId}", 
            quantity, stationId, fuelTypeId);

        var stock = await GetStockAsync(stationId, fuelTypeId, cancellationToken);

        if (stock == null)
        {
            throw new InvalidOperationException($"Fuel stock not found for station {stationId}, fuel type {fuelTypeId}");
        }

        if (stock.CurrentQuantity < quantity)
        {
            throw new InvalidOperationException($"Insufficient stock. Cannot deduct {quantity} litres from {stock.CurrentQuantity} litres available");
        }

        var stockBefore = stock.CurrentQuantity;
        var stockAfter = stock.CurrentQuantity - quantity;

        // Update stock
        stock.CurrentQuantity = stockAfter;
        stock.LastUpdated = DateTime.UtcNow;
        stock.IsLowStock = stock.LowStockThreshold.HasValue && stock.CurrentQuantity <= stock.LowStockThreshold.Value;

        // Create StockMovement record
        var stockMovement = new StockMovement
        {
            OrganisationId = stock.OrganisationId,
            FuelStationId = stationId,
            FuelTypeId = fuelTypeId,
            MovementType = "Dispense",
            Quantity = quantity,
            Unit = stock.Unit,
            StockBefore = stockBefore,
            StockAfter = stockAfter,
            ReferenceNumber = referenceNumber,
            MovementDate = DateTime.UtcNow,
            CreatorId = creatorId
        };

        _context.StockMovements.Add(stockMovement);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deducted {Quantity} litres from stock. Stock: {StockBefore} -> {StockAfter}", 
            quantity, stockBefore, stockAfter);

        return stock;
    }

    public async Task<decimal> GetMaxDispenseableQuantityAsync(int stationId, int fuelTypeId, CancellationToken cancellationToken = default)
    {
        var stock = await GetStockAsync(stationId, fuelTypeId, cancellationToken);

        if (stock == null || stock.CurrentQuantity <= 0)
        {
            return 0;
        }

        return stock.CurrentQuantity;
    }
}

