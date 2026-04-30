using System;
using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize(Roles = AppRoles.PumpOperatorRoles)]
public class FuelPumpController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly ILogger<FuelPumpController> _logger;

    public FuelPumpController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        ILogger<FuelPumpController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: FuelPump
    public async Task<IActionResult> Index(int? stationId)
    {
        IQueryable<FuelPump> query = _context.FuelPumps
            .Include(p => p.FuelStation)
            .Include(p => p.FuelType);

        if (stationId.HasValue)
        {
            query = query.Where(p => p.FuelStationId == stationId.Value);
        }

        var pumps = await query.OrderBy(p => p.FuelStation.Name).ThenBy(p => p.PumpNumber).ToListAsync();
        
        if (stationId.HasValue)
        {
            var station = await _context.FuelStations.FindAsync(stationId.Value);
            ViewBag.StationId = stationId.Value;
            ViewBag.StationName = station?.Name;
        }

        return View(pumps);
    }

    // GET: FuelPump/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pump = await _context.FuelPumps
            .Include(p => p.FuelStation)
            .Include(p => p.FuelType)
            .Include(p => p.Organisation)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pump == null)
        {
            return NotFound();
        }

        ViewBag.StationId = pump.FuelStationId;
        ViewBag.StationName = pump.FuelStation?.Name;
        return View(pump);
    }

    // Managers can only toggle operational status (deactivate/activate).
    // Create and Edit are not permitted — pump hardware config is read-only.

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleOperational(int id)
    {
        var pump = await _context.FuelPumps.FindAsync(id);
        if (pump == null)
        {
            return NotFound();
        }

        pump.IsOperational = !pump.IsOperational;
        pump.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { stationId = pump.FuelStationId });
    }

    private async Task<bool> FuelPumpExistsAsync(int id)
    {
        return await _context.FuelPumps.AnyAsync(e => e.Id == id);
    }
}

