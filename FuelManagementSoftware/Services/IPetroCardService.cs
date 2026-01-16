using System.Threading;
using System.Threading.Tasks;
using FuelManagementSoftware.Models;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service interface for PetroCard operations including validation, balance management, and PIN verification.
/// </summary>
public interface IPetroCardService
{
    /// <summary>
    /// Finds a PetroCard by its NFC tag UID.
    /// </summary>
    /// <param name="nfcTag">The NFC tag UID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The PetroCard if found, null otherwise</returns>
    Task<PetroCard?> GetCardByNfcTagAsync(string nfcTag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a PetroCard for use in a transaction.
    /// Checks if card is active, not blocked, not expired, and has sufficient balance.
    /// </summary>
    /// <param name="card">The PetroCard to validate</param>
    /// <param name="requiredAmount">The amount required for the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with success status and error message if failed</returns>
    Task<CardValidationResult> ValidateCardAsync(PetroCard card, decimal requiredAmount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the PIN for a PetroCard.
    /// </summary>
    /// <param name="card">The PetroCard</param>
    /// <param name="pin">The PIN to verify</param>
    /// <returns>True if PIN is correct, false otherwise</returns>
    bool VerifyPin(PetroCard card, string pin);

    /// <summary>
    /// Deducts an amount from the card balance and creates a CardTransaction record.
    /// </summary>
    /// <param name="cardId">The ID of the PetroCard</param>
    /// <param name="amount">The amount to deduct</param>
    /// <param name="transactionType">Type of transaction (e.g., "FuelPurchase", "Refund")</param>
    /// <param name="referenceNumber">Optional reference number for the transaction</param>
    /// <param name="creatorId">The ID of the user/system creating this transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated PetroCard with new balance</returns>
    Task<PetroCard> DeductBalanceAsync(int cardId, decimal amount, string transactionType, string? referenceNumber, string creatorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the LastUsedAt timestamp for a card.
    /// </summary>
    /// <param name="cardId">The ID of the PetroCard</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateLastUsedAsync(int cardId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hashes a PIN using BCrypt for secure storage.
    /// </summary>
    /// <param name="pin">The PIN to hash</param>
    /// <returns>The hashed PIN</returns>
    string HashPin(string pin);
}

/// <summary>
/// Result of card validation operation.
/// </summary>
public class CardValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public static CardValidationResult Success() => new() { IsValid = true };
    public static CardValidationResult Failure(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };
}

