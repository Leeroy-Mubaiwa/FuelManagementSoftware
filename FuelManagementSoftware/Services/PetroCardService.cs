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
/// Service for managing PetroCard operations including validation, balance management, and PIN verification.
/// </summary>
public class PetroCardService : IPetroCardService
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly ILogger<PetroCardService> _logger;

    public PetroCardService(FilteredFuelManagementSoftwareDbContext context, ILogger<PetroCardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PetroCard?> GetCardByNfcTagAsync(string nfcTag, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nfcTag))
        {
            return null;
        }

        _logger.LogDebug("Looking up PetroCard by NFC tag: {NfcTag}", nfcTag);

        var card = await _context.PetroCards
            .Include(c => c.User)
            .Include(c => c.Organisation)
            .FirstOrDefaultAsync(c => c.Rfidtag == nfcTag && c.IsActive, cancellationToken);

        if (card == null)
        {
            _logger.LogWarning("PetroCard not found for NFC tag: {NfcTag}", nfcTag);
        }

        return card;
    }

    public async Task<CardValidationResult> ValidateCardAsync(PetroCard card, decimal requiredAmount, CancellationToken cancellationToken = default)
    {
        if (card == null)
        {
            return CardValidationResult.Failure("Card not found");
        }

        // Check if card is active
        if (!card.IsActive)
        {
            _logger.LogWarning("Card {CardId} ({CardNumber}) is not active", card.Id, card.CardNumber);
            return CardValidationResult.Failure("Card is not active");
        }

        // Check if card is blocked
        if (card.IsBlocked)
        {
            _logger.LogWarning("Card {CardId} ({CardNumber}) is blocked", card.Id, card.CardNumber);
            return CardValidationResult.Failure("Card is blocked");
        }

        // Check if card is expired
        if (card.ExpiryDate.HasValue && card.ExpiryDate.Value < DateTime.UtcNow)
        {
            _logger.LogWarning("Card {CardId} ({CardNumber}) has expired", card.Id, card.CardNumber);
            return CardValidationResult.Failure("Card has expired");
        }

        // Check if card has sufficient balance
        if (card.Balance < requiredAmount)
        {
            _logger.LogWarning("Card {CardId} ({CardNumber}) has insufficient balance. Required: {Required}, Available: {Balance}", 
                card.Id, card.CardNumber, requiredAmount, card.Balance);
            return CardValidationResult.Failure($"Insufficient balance. Required: {requiredAmount:F2} {card.Currency}, Available: {card.Balance:F2} {card.Currency}");
        }

        _logger.LogDebug("Card {CardId} ({CardNumber}) validation successful", card.Id, card.CardNumber);
        return CardValidationResult.Success();
    }

    public bool VerifyPin(PetroCard card, string pin)
    {
        if (card == null || string.IsNullOrWhiteSpace(pin))
        {
            return false;
        }

        // If card has no PIN hash, PIN verification is not required
        if (string.IsNullOrWhiteSpace(card.PinHash))
        {
            _logger.LogDebug("Card {CardId} has no PIN set, PIN verification skipped", card.Id);
            return true;
        }

        // Verify PIN using BCrypt
        try
        {
            var isValid = BCrypt.Net.BCrypt.Verify(pin, card.PinHash);
            
            if (!isValid)
            {
                _logger.LogWarning("Invalid PIN provided for card {CardId}", card.Id);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying PIN for card {CardId}", card.Id);
            return false;
        }
    }

    /// <summary>
    /// Hashes a PIN using BCrypt for secure storage.
    /// </summary>
    public static string HashPin(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin))
        {
            throw new ArgumentException("PIN cannot be null or empty", nameof(pin));
        }

        // BCrypt automatically generates a salt and includes it in the hash
        return BCrypt.Net.BCrypt.HashPassword(pin, BCrypt.Net.BCrypt.GenerateSalt());
    }

    public async Task<PetroCard> DeductBalanceAsync(int cardId, decimal amount, string transactionType, string? referenceNumber, string creatorId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deducting {Amount} from card {CardId} for transaction type {TransactionType}", 
            amount, cardId, transactionType);

        var card = await _context.PetroCards
            .FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);

        if (card == null)
        {
            throw new InvalidOperationException($"PetroCard with ID {cardId} not found");
        }

        var balanceBefore = card.Balance;
        var balanceAfter = card.Balance - amount;

        if (balanceAfter < 0)
        {
            throw new InvalidOperationException($"Insufficient balance. Cannot deduct {amount} from balance {balanceBefore}");
        }

        // Update card balance
        card.Balance = balanceAfter;
        card.UpdatedAt = DateTime.UtcNow;
        card.LastUsedAt = DateTime.UtcNow;

        // Create CardTransaction record
        var cardTransaction = new CardTransaction
        {
            OrganisationId = card.OrganisationId,
            PetroCardId = cardId,
            TransactionType = transactionType,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = balanceAfter,
            Currency = card.Currency,
            ReferenceNumber = referenceNumber,
            Description = $"Balance deduction for {transactionType}",
            TransactionDate = DateTime.UtcNow,
            CreatorId = creatorId
        };

        _context.CardTransactions.Add(cardTransaction);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deducted {Amount} from card {CardId}. Balance: {BalanceBefore} -> {BalanceAfter}", 
            amount, cardId, balanceBefore, balanceAfter);

        return card;
    }

    public async Task UpdateLastUsedAsync(int cardId, CancellationToken cancellationToken = default)
    {
        var card = await _context.PetroCards
            .FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);

        if (card != null)
        {
            card.LastUsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogDebug("Updated LastUsedAt for card {CardId}", cardId);
        }
    }

    string IPetroCardService.HashPin(string pin)
    {
        return PetroCardService.HashPin(pin);
    }
}

