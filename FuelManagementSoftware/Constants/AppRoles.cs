namespace FuelManagementSoftware.Constants;

/// <summary>
/// Application role names. All roles except Customer are organisation employees.
/// Customer is assigned when a user registers as Customer; others are assigned by admins.
/// </summary>
public static class AppRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string SystemAdmin = "SystemAdmin";
    public const string OrganizationAdmin = "OrganizationAdmin";
    public const string OrganizationManager = "OrganizationManager";
    public const string StationManager = "StationManager";
    public const string StationOperator = "StationOperator";
    public const string FuelManager = "FuelManager";
    public const string FuelOperator = "FuelOperator";
    public const string CardManager = "CardManager";
    public const string CardOperator = "CardOperator";
    public const string CustomerService = "CustomerService";
    public const string ReportViewer = "ReportViewer";
    public const string Auditor = "Auditor";
    public const string Maintenance = "Maintenance";
    public const string Customer = "Customer";
    public const string User = "User";

    /// <summary>
    /// Comma-separated list of all roles that are under an organisation (employees).
    /// Used for [Authorize(Roles = ...)] on Management controllers.
    /// </summary>
    public const string OrganisationRoles = "SuperAdmin,SystemAdmin,OrganizationAdmin,OrganizationManager,StationManager,StationOperator,FuelManager,FuelOperator,CardManager,CardOperator,CustomerService,ReportViewer,Auditor,Maintenance,User";
}
