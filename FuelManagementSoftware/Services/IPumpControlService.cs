using System.Threading;
using System.Threading.Tasks;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service interface for controlling fuel pump hardware.
/// Handles starting, stopping, and monitoring fuel pumps.
/// </summary>
public interface IPumpControlService
{
    /// <summary>
    /// Starts fuel dispensing on the specified pump.
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if pump started successfully, false otherwise</returns>
    Task<bool> StartPumpAsync(int pumpId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops fuel dispensing on the specified pump.
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if pump stopped successfully, false otherwise</returns>
    Task<bool> StopPumpAsync(int pumpId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current quantity dispensed from the pump (in litres).
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current quantity dispensed, or null if pump is not active</returns>
    Task<decimal?> GetDispensedQuantityAsync(int pumpId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the pump is currently dispensing.
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if pump is dispensing, false otherwise</returns>
    Task<bool> IsPumpActiveAsync(int pumpId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the pump meter/display.
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if reset successful, false otherwise</returns>
    Task<bool> ResetPumpAsync(int pumpId, CancellationToken cancellationToken = default);
}

