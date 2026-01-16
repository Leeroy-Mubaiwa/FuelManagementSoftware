using System.Threading;
using System.Threading.Tasks;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service interface for NFC reader operations.
/// Handles reading NFC tags from physical NFC readers at fuel pumps.
/// </summary>
public interface INfcReaderService
{
    /// <summary>
    /// Reads the NFC tag UID from the reader.
    /// Returns null if no tag is present or if read fails.
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump where the NFC reader is located</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The NFC tag UID (unique identifier) or null if no tag detected</returns>
    Task<string?> ReadNfcTagAsync(int pumpId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an NFC tag is currently present on the reader.
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump where the NFC reader is located</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if a tag is present, false otherwise</returns>
    Task<bool> IsNfcTagPresentAsync(int pumpId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits asynchronously for an NFC tag to be tapped on the reader.
    /// This method will wait until a tag is detected or the cancellation token is triggered.
    /// </summary>
    /// <param name="pumpId">The ID of the fuel pump where the NFC reader is located</param>
    /// <param name="timeoutMs">Maximum time to wait in milliseconds (default: 30000 = 30 seconds)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The NFC tag UID when detected, or null if timeout or cancelled</returns>
    Task<string?> WaitForNfcTagAsync(int pumpId, int timeoutMs = 30000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that the NFC tag format is correct and authentic.
    /// </summary>
    /// <param name="nfcTag">The NFC tag UID to validate</param>
    /// <returns>True if the tag format is valid, false otherwise</returns>
    bool ValidateNfcTag(string nfcTag);
}

