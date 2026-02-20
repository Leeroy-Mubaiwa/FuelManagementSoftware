using System.Threading.Tasks;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FuelManagementSoftware.Services;

/// <summary>
/// Seeds global fuel types (hardcoded). Fuel types are system-wide, not per organisation.
/// </summary>
public class FuelTypeSeederService
{
    private readonly FuelManagementSoftwareDbContext _context;
    private readonly ILogger<FuelTypeSeederService> _logger;

    public FuelTypeSeederService(FuelManagementSoftwareDbContext context, ILogger<FuelTypeSeederService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedFuelTypesAsync()
    {
        var defaults = new[]
        {
            new { Name = "Petrol", Code = "PETROL", Unit = "Litre", UnitPrice = 0m },
            new { Name = "Diesel", Code = "DIESEL", Unit = "Litre", UnitPrice = 0m },
            new { Name = "Premium Petrol", Code = "PREMIUM", Unit = "Litre", UnitPrice = 0m }
        };

        foreach (var d in defaults)
        {
            var exists = await _context.FuelTypes.AnyAsync(ft => ft.Code == d.Code || ft.Name == d.Name);
            if (exists) continue;

            _context.FuelTypes.Add(new FuelType
            {
                Name = d.Name,
                Code = d.Code,
                Unit = d.Unit,
                UnitPrice = d.UnitPrice,
                IsActive = true,
                CreatorId = null,
                CreatedAt = System.DateTime.UtcNow
            });
            _logger.LogInformation("Seeded global fuel type: {Name}", d.Name);
        }

        await _context.SaveChangesAsync();
    }
}
