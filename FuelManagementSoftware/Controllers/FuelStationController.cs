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
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize(Roles = AppRoles.OrganisationRoles)]
public class FuelStationController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly IOrganisationContextService _organisationContext;
    private readonly ILogger<FuelStationController> _logger;

    public FuelStationController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        IOrganisationContextService organisationContext,
        ILogger<FuelStationController> logger)
    {
        _context = context;
        _userManager = userManager;
        _organisationContext = organisationContext;
        _logger = logger;
    }

    // GET: FuelStation
    public async Task<IActionResult> Index()
    {
        var stations = await _context.FuelStations
            .Include(s => s.Organisation)
            .OrderBy(s => s.Name)
            .ToListAsync();
        return View(stations);
    }

    // GET: FuelStation/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var station = await _context.FuelStations
            .Include(s => s.Organisation)
            .Include(s => s.FuelPumps)
            .ThenInclude(p => p.FuelType)
            .Include(s => s.FuelStocks)
            .ThenInclude(st => st.FuelType)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (station == null)
        {
            return NotFound();
        }

        ViewBag.StationId = station.Id;
        ViewBag.StationName = station.Name;
        return View(station);
    }

    // GET: FuelStation/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: FuelStation/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Code,Address,City,Latitude,Longitude,Phone,Email,IsActive,IsOpen,IsTankerOffloading")] FuelStation station)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var organisationId = await _organisationContext.GetCurrentOrganisationIdAsync();
            if (!organisationId.HasValue) return BadRequest("Organization not found");

            station.OrganisationId = organisationId.Value;
            station.CreatorId = user.Id;
            station.CreatedAt = DateTime.UtcNow;

            _context.FuelStations.Add(station);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(station);
    }

    // GET: FuelStation/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var station = await _context.FuelStations.FindAsync(id);
        if (station == null)
        {
            return NotFound();
        }
        return View(station);
    }

    // POST: FuelStation/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,OrganisationId,Name,Code,Address,City,Latitude,Longitude,Phone,Email,IsActive,IsOpen,IsTankerOffloading")] FuelStation station)
    {
        if (id != station.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existing = await _context.FuelStations.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Name = station.Name;
                existing.Code = station.Code;
                existing.Address = station.Address;
                existing.City = station.City;
                existing.Latitude = station.Latitude;
                existing.Longitude = station.Longitude;
                existing.Phone = station.Phone;
                existing.Email = station.Email;
                existing.IsActive = station.IsActive;
                existing.IsOpen = station.IsOpen;
                existing.IsTankerOffloading = station.IsTankerOffloading;
                existing.OrganisationId = station.OrganisationId;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FuelStationExistsAsync(station.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        return View(station);
    }

    // POST: FuelStation/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var station = await _context.FuelStations.FindAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        station.IsOpen = !station.IsOpen;
        station.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<bool> FuelStationExistsAsync(int id)
    {
        return await _context.FuelStations.AnyAsync(e => e.Id == id);
    }

}

