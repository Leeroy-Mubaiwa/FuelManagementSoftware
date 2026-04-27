using System;
using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Seeds the default Petrotrade organization for single-tenant mode.
/// </summary>
public class OrganizationSeederService
{
    private readonly FuelManagementSoftwareDbContext _context;
    private readonly ILogger<OrganizationSeederService> _logger;

    public OrganizationSeederService(FuelManagementSoftwareDbContext context, ILogger<OrganizationSeederService> _logger)
    {
        _context = context;
        this._logger = _logger;
    }

    public async Task SeedOrganizationAsync()
    {
        var exists = await _context.Organizations.AnyAsync();
        if (exists)
        {
            _logger.LogDebug("Organization already exists, skipping seeding.");
            return;
        }

        var org = new Organization
        {
            Name = "Petrotrade",
            Code = "PETROTRADE",
            Address = "123 Fuel Way, Harare",
            Email = "info@petrotrade.co.zw",
            Phone = "+263 242 123456",
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Organizations.Add(org);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Default organization 'Petrotrade' seeded successfully.");
    }
}
