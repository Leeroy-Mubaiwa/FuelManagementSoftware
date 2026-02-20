using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize(Roles = AppRoles.OrganisationRoles)]
public class FuelTypeController : Controller
{
    private readonly FuelManagementSoftwareDbContext _context;

    public FuelTypeController(FuelManagementSoftwareDbContext context)
    {
        _context = context;
    }

    // GET: FuelType (global list, read-only; types are seeded)
    public async Task<IActionResult> Index()
    {
        var fuelTypes = await _context.FuelTypes
            .OrderBy(ft => ft.Name)
            .ToListAsync();
        return View(fuelTypes);
    }

    // GET: FuelType/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var fuelType = await _context.FuelTypes.FirstOrDefaultAsync(ft => ft.Id == id);
        if (fuelType == null) return NotFound();

        return View(fuelType);
    }
}

