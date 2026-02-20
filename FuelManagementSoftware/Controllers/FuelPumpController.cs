using System;
using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using FuelManagementSoftware.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize(Roles = AppRoles.OrganisationRoles)]
public class FuelPumpController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly IOrganisationContextService _organisationContext;
    private readonly ILogger<FuelPumpController> _logger;

    public FuelPumpController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        IOrganisationContextService organisationContext,
        ILogger<FuelPumpController> logger)
    {
        _context = context;
        _userManager = userManager;
        _organisationContext = organisationContext;
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

        return View(pump);
    }

    // GET: FuelPump/Create
    public async Task<IActionResult> Create(int? stationId)
    {
        var stations = await _context.FuelStations
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var fuelTypes = await _context.FuelTypes
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Name)
            .ToListAsync();

        ViewBag.FuelStationId = new SelectList(stations, "Id", "Name", stationId);
        ViewBag.FuelTypeId = new SelectList(fuelTypes, "Id", "Name");
        return View();
    }

    // POST: FuelPump/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FuelStationId,PumpNumber,FuelTypeId,IsActive,IsOperational,LastMaintenanceDate")] FuelPump pump)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var organisationId = await _organisationContext.GetCurrentOrganisationIdAsync();
            if (!organisationId.HasValue) return BadRequest("Organization not found");

            pump.OrganisationId = organisationId.Value;
            pump.CreatorId = user.Id;
            pump.CreatedAt = DateTime.UtcNow;

            _context.FuelPumps.Add(pump);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { stationId = pump.FuelStationId });
        }

        var stations = await _context.FuelStations
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var fuelTypes = await _context.FuelTypes
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Name)
            .ToListAsync();

        ViewBag.FuelStationId = new SelectList(stations, "Id", "Name", pump.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(fuelTypes, "Id", "Name", pump.FuelTypeId);
        return View(pump);
    }

    // GET: FuelPump/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pump = await _context.FuelPumps.FindAsync(id);
        if (pump == null)
        {
            return NotFound();
        }

        var stations = await _context.FuelStations
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var fuelTypes = await _context.FuelTypes
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Name)
            .ToListAsync();

        ViewBag.FuelStationId = new SelectList(stations, "Id", "Name", pump.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(fuelTypes, "Id", "Name", pump.FuelTypeId);
        return View(pump);
    }

    // POST: FuelPump/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,OrganisationId,FuelStationId,PumpNumber,FuelTypeId,IsActive,IsOperational,LastMaintenanceDate")] FuelPump pump)
    {
        if (id != pump.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                pump.UpdatedAt = DateTime.UtcNow;
                _context.Update(pump);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { stationId = pump.FuelStationId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FuelPumpExistsAsync(pump.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        var stations = await _context.FuelStations
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var fuelTypes = await _context.FuelTypes
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Name)
            .ToListAsync();

        ViewBag.FuelStationId = new SelectList(stations, "Id", "Name", pump.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(fuelTypes, "Id", "Name", pump.FuelTypeId);
        return View(pump);
    }

    private async Task<bool> FuelPumpExistsAsync(int id)
    {
        return await _context.FuelPumps.AnyAsync(e => e.Id == id);
    }
}

