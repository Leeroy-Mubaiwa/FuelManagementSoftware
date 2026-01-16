using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Models;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service interface for automated fuel dispensing operations.
/// Orchestrates the complete fuel dispensing workflow from card authentication to completion.
/// </summary>
public interface IFuelDispensingService
{
    /// <summary>
    /// Initiates a fuel dispensing transaction.
    /// Validates card, checks stock, and creates a pending transaction.
    /// </summary>
    /// <param name="request">The dispensing request containing pump, card, and fuel type information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created FuelTransaction in "Authorized" or "Pending" status</returns>
    Task<FuelTransaction> InitiateDispensingAsync(DispensingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the actual fuel dispensing process.
    /// Sends start command to pump and begins monitoring.
    /// </summary>
    /// <param name="transactionId">The ID of the authorized transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated FuelTransaction in "Dispensing" status</returns>
    Task<FuelTransaction> StartDispensingAsync(int transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the transaction with current dispensing progress (quantity dispensed).
    /// </summary>
    /// <param name="transactionId">The ID of the active transaction</param>
    /// <param name="quantityDispensed">The current quantity dispensed (in litres)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated FuelTransaction</returns>
    Task<FuelTransaction> UpdateDispensingProgressAsync(int transactionId, decimal quantityDispensed, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a fuel dispensing transaction.
    /// Stops the pump, deducts balance and stock, and finalizes the transaction.
    /// </summary>
    /// <param name="transactionId">The ID of the active transaction</param>
    /// <param name="finalQuantity">The final quantity dispensed (in litres)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The completed FuelTransaction</returns>
    Task<FuelTransaction> CompleteDispensingAsync(int transactionId, decimal finalQuantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an active or pending fuel dispensing transaction.
    /// </summary>
    /// <param name="transactionId">The ID of the transaction to cancel</param>
    /// <param name="reason">Optional reason for cancellation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cancelled FuelTransaction</returns>
    Task<FuelTransaction> CancelDispensingAsync(int transactionId, string? reason = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request model for initiating fuel dispensing.
/// </summary>
public class DispensingRequest
{
    public int FuelStationId { get; set; }
    public int FuelPumpId { get; set; }
    public int FuelTypeId { get; set; }
    public string NfcTag { get; set; } = null!;
    public string? Pin { get; set; }
    public decimal? RequestedQuantity { get; set; } // Optional: if null, customer can dispense until they stop or balance/stock runs out
    public string CreatorId { get; set; } = null!;
}

