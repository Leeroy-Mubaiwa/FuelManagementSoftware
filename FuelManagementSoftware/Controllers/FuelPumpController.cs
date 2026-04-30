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

[Authorize(Roles = AppRoles.OrganisationRoles)]
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

    // GET: FuelPump/Create
    [Authorize(Roles = AppRoles.BranchStationManager)]
    public async Task<IActionResult> Create(int? stationId)
    {
        ViewBag.FuelStationId = new SelectList(await _context.FuelStations.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", stationId);
        ViewBag.FuelTypeId = new SelectList(await _context.FuelTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name");
        return View();
    }

    // POST: FuelPump/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.BranchStationManager)]
    public async Task<IActionResult> Create([Bind("FuelStationId,PumpNumber,FuelTypeId,IsActive,IsOperational,LastMaintenanceDate")] FuelPump fuelPump)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            fuelPump.CreatorId = user?.Id;
            fuelPump.CreatedAt = DateTime.Now;
            
            _context.FuelPumps.Add(fuelPump);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { stationId = fuelPump.FuelStationId });
        }
        ViewBag.FuelStationId = new SelectList(await _context.FuelStations.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", fuelPump.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(await _context.FuelTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", fuelPump.FuelTypeId);
        return View(fuelPump);
    }

    // GET: FuelPump/Edit/5
    [Authorize(Roles = AppRoles.BranchStationManager)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var fuelPump = await _context.FuelPumps.FindAsync(id);
        if (fuelPump == null) return NotFound();

        ViewBag.FuelStationId = new SelectList(await _context.FuelStations.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", fuelPump.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(await _context.FuelTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", fuelPump.FuelTypeId);
        return View(fuelPump);
    }

    // POST: FuelPump/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.BranchStationManager)]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FuelStationId,PumpNumber,FuelTypeId,IsActive,IsOperational,LastMaintenanceDate,CreatedAt,CreatorId")] FuelPump fuelPump)
    {
        if (id != fuelPump.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                fuelPump.UpdatedAt = DateTime.Now;
                _context.Update(fuelPump);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FuelPumpExistsAsync(fuelPump.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index), new { stationId = fuelPump.FuelStationId });
        }
        ViewBag.FuelStationId = new SelectList(await _context.FuelStations.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", fuelPump.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(await _context.FuelTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", fuelPump.FuelTypeId);
        return View(fuelPump);
    }

    // POST: FuelPump/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.BranchStationManager)]
    public async Task<IActionResult> Delete(int id)
    {
        var fuelPump = await _context.FuelPumps.FindAsync(id);
        if (fuelPump == null) return NotFound();

        var stationId = fuelPump.FuelStationId;
        _context.FuelPumps.Remove(fuelPump);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { stationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.BranchStationManager)]
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

