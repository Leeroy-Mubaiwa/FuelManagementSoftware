namespace FuelManagementSoftware.Constants;

/// <summary>
/// Application role names. All roles except Customer are organisation employees.
/// Customer is assigned when a user registers as Customer; others are assigned by admins.
/// </summary>
public static class AppRoles
{
    public const string PetrotradeAdmin = "PetrotradeAdmin";
    public const string BranchStationManager = "BranchStationManager";
    public const string Customer = "Customer";

    /// <summary>
    /// Comma-separated list of all roles that are under an organisation (employees).
    /// Used for [Authorize(Roles = ...)] on Management controllers.
    /// </summary>
    public const string OrganisationRoles = "PetrotradeAdmin,BranchStationManager";
}
