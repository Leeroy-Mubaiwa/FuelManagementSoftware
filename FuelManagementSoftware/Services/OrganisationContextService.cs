using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service to get the current organisation ID from the authenticated user.
/// Retrieves organisation ID from:
/// 1. User claims (if set during login)
/// 2. User's PetroCard (first card they own)
/// 3. User's created Organization (if they created one)
/// </summary>
public class OrganisationContextService : IOrganisationContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly ILogger<OrganisationContextService> _logger;

    public OrganisationContextService(
        IHttpContextAccessor httpContextAccessor,
        FuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        ILogger<OrganisationContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<int?> GetCurrentOrganisationIdAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.User.Identity?.IsAuthenticated == true)
        {
            return null;
        }

        var user = httpContext.User;
        var userId = _userManager.GetUserId(user);

        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        // 1. Try to get from user claims first
        var organisationIdClaim = user.FindFirst("OrganisationId")?.Value;
        if (!string.IsNullOrEmpty(organisationIdClaim) && int.TryParse(organisationIdClaim, out var orgIdFromClaim))
        {
            _logger.LogDebug("Organisation ID found in claims: {OrganisationId}", orgIdFromClaim);
            return orgIdFromClaim;
        }

        // 2. Try to get from user's PetroCard
        var userCard = await _context.PetroCards
            .Where(c => c.UserId == userId && c.IsActive)
            .OrderByDescending(c => c.LastUsedAt ?? c.CreatedAt)
            .FirstOrDefaultAsync();

        if (userCard != null)
        {
            _logger.LogDebug("Organisation ID found from user's PetroCard: {OrganisationId}", userCard.OrganisationId);
            return userCard.OrganisationId;
        }

        // 3. Try to get from user's created Organization (if they created one)
        var userOrg = await _context.Organizations
            .Where(o => o.CreatorId == userId && o.IsActive)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (userOrg != null)
        {
            _logger.LogDebug("Organisation ID found from user's created organization: {OrganisationId}", userOrg.Id);
            return userOrg.Id;
        }

        // 3.5 Try to get from user's fuel transactions (useful for attendants/operators).
        var userTransactionOrg = await _context.FuelTransactions
            .Where(t => t.CreatorId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => (int?)t.OrganisationId)
            .FirstOrDefaultAsync();

        if (userTransactionOrg.HasValue)
        {
            _logger.LogDebug("Organisation ID found from user's FuelTransaction records: {OrganisationId}", userTransactionOrg.Value);
            return userTransactionOrg.Value;
        }

        // 4. Fallback for admin roles: use first active organisation (e.g. seeded org or single-tenant)
        var fuelUser = await _userManager.GetUserAsync(user);
        if (fuelUser != null)
        {
            var isAdmin = await _userManager.IsInRoleAsync(fuelUser, AppRoles.PetrotradeAdmin);
            if (isAdmin)
            {
                var firstOrg = await _context.Organizations
                    .Where(o => o.IsActive)
                    .OrderBy(o => o.Id)
                    .Select(o => o.Id)
                    .FirstOrDefaultAsync();
                if (firstOrg != 0)
                {
                    _logger.LogDebug("Organisation ID fallback for admin: {OrganisationId}", firstOrg);
                    return firstOrg;
                }
            }
        }

        _logger.LogWarning("No organisation ID found for user {UserId} - global filtering will be disabled", userId);
        return null;
    }
}

