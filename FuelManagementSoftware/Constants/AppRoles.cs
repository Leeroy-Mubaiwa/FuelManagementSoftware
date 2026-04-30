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

    /// <summary>
    /// Roles allowed to perform fuel dispensing (Manager at the pump + Customer).
    /// Admin is explicitly excluded – they should never dispense fuel.
    /// </summary>
    public const string DispensingRoles = "BranchStationManager,Customer";

    /// <summary>
    /// Roles that operate and manage physical pump hardware.
    /// Only Branch/Station Managers who are physically at the station.
    /// Admin cannot tamper with pumps.
    /// </summary>
    public const string PumpOperatorRoles = "BranchStationManager";

    /// <summary>
    /// Roles that can manage fuel stock (receive deliveries, adjust quantities).
    /// Scoped to branch managers only — one manager per station.
    /// </summary>
    public const string StockManagementRoles = "BranchStationManager";
}
