using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Models;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service interface for fuel stock management operations.
/// </summary>
public interface IFuelStockService
{
    /// <summary>
    /// Gets the current fuel stock for a specific station and fuel type.
    /// </summary>
    /// <param name="stationId">The fuel station ID</param>
    /// <param name="fuelTypeId">The fuel type ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The FuelStock record, or null if not found</returns>
    Task<FuelStock?> GetStockAsync(int stationId, int fuelTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there is sufficient stock available for a transaction.
    /// </summary>
    /// <param name="stationId">The fuel station ID</param>
    /// <param name="fuelTypeId">The fuel type ID</param>
    /// <param name="requiredQuantity">The quantity required (in litres)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sufficient stock is available, false otherwise</returns>
    Task<bool> HasSufficientStockAsync(int stationId, int fuelTypeId, decimal requiredQuantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deducts fuel stock after a transaction.
    /// </summary>
    /// <param name="stationId">The fuel station ID</param>
    /// <param name="fuelTypeId">The fuel type ID</param>
    /// <param name="quantity">The quantity to deduct (in litres)</param>
    /// <param name="creatorId">The ID of the user/system creating this stock movement</param>
    /// <param name="referenceNumber">Optional reference number (e.g., transaction number)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated FuelStock record</returns>
    Task<FuelStock> DeductStockAsync(int stationId, int fuelTypeId, decimal quantity, string creatorId, string? referenceNumber = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the maximum quantity that can be dispensed based on available stock.
    /// </summary>
    /// <param name="stationId">The fuel station ID</param>
    /// <param name="fuelTypeId">The fuel type ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The maximum dispenseable quantity, or 0 if no stock available</returns>
    Task<decimal> GetMaxDispenseableQuantityAsync(int stationId, int fuelTypeId, CancellationToken cancellationToken = default);
}

