using System.Threading.Tasks;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service to get the current organisation ID for the current request/user context.
/// This is used by the FilteredFuelManagementSoftwareDbContext to apply organisation filtering.
/// </summary>
public interface IOrganisationContextService
{
    /// <summary>
    /// Gets the current organisation ID for the current request context.
    /// This could be from the authenticated user, request header, or other context.
    /// </summary>
    /// <returns>The organisation ID, or null if not available</returns>
    Task<int?> GetCurrentOrganisationIdAsync();
}

