using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Constants;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FuelManagementSoftware.Services;

public class DefaultUserSeederService
{
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly ILogger<DefaultUserSeederService> _logger;
    private readonly FuelManagementSoftware.Data.FuelManagementSoftwareDbContext _dbContext;

    public DefaultUserSeederService(
        UserManager<FuelManagementSoftwareUser> userManager, 
        ILogger<DefaultUserSeederService> logger,
        FuelManagementSoftware.Data.FuelManagementSoftwareDbContext dbContext)
    {
        _userManager = userManager;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SeedUsersAsync()
    {
        var petrotradeOrg = _dbContext.Organizations.FirstOrDefault(o => o.Name == "Petrotrade");
        string? orgIdString = petrotradeOrg?.Id.ToString();

        await SeedUserAsync("admin@petrotrade.co.zw", "Admin@123", AppRoles.PetrotradeAdmin, orgIdString);
        await SeedUserAsync("manager@petrotrade.co.zw", "Manager@123", AppRoles.BranchStationManager, orgIdString);
        await SeedUserAsync("customer@gmail.com", "Customer@123", AppRoles.Customer, null);
    }

    private async Task SeedUserAsync(string email, string password, string role, string? orgId)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing == null)
        {
            var user = new FuelManagementSoftwareUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                
                if (!string.IsNullOrEmpty(orgId))
                {
                    await _userManager.AddClaimAsync(user, new Claim("OrganisationId", orgId));
                }

                _logger.LogInformation("Seeded user {Email} with role {Role}", email, role);
            }
            else
            {
                _logger.LogError("Failed to seed user {Email}: {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
