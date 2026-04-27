using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using FuelManagementSoftware.Constants;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Service to seed application roles using Identity Role Manager.
/// Creates all roles needed for the fuel management system.
/// </summary>
public class RoleSeederService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RoleSeederService> _logger;

    public RoleSeederService(RoleManager<IdentityRole> roleManager, ILogger<RoleSeederService> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Seeds all application roles if they don't already exist.
    /// </summary>
    public async Task SeedRolesAsync()
    {
        var roles = new List<string>
        {
            AppRoles.PetrotradeAdmin,
            AppRoles.BranchStationManager,
            AppRoles.Customer
        };

        foreach (var roleName in roles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            
            if (!roleExists)
            {
                var role = new IdentityRole(roleName)
                {
                    NormalizedName = roleName.ToUpperInvariant()
                };
                
                var result = await _roleManager.CreateAsync(role);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Role '{RoleName}' created successfully", roleName);
                }
                else
                {
                    _logger.LogError("Failed to create role '{RoleName}': {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogDebug("Role '{RoleName}' already exists", roleName);
            }
        }

        _logger.LogInformation("Role seeding completed");
    }
}

