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
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize(Roles = AppRoles.OrganisationRoles)]
public class FuelStationController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly ILogger<FuelStationController> _logger;

    public FuelStationController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        ILogger<FuelStationController> logger)
    {
        _context = context;
        _userManager = userManager;
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
    public async Task<IActionResult> Create()
    {
        await PopulateManagersSelectList();
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

            station.CreatorId = user.Id;
            station.CreatedAt = DateTime.Now;

            _context.FuelStations.Add(station);
            await _context.SaveChangesAsync();

            // Handle manager assignment (one primary manager per station via this UI)
            var managerId = Request.Form["ManagerId"].ToString();
            
            // First, clear any other managers currently assigned to this station if we are setting a new one or clearing
            var otherManagers = await _userManager.Users.Where(u => u.ManagedStationId == station.Id).ToListAsync();
            foreach (var m in otherManagers)
            {
                m.ManagedStationId = null;
                await _userManager.UpdateAsync(m);
            }

            if (!string.IsNullOrEmpty(managerId))
            {
                var manager = await _userManager.FindByIdAsync(managerId);
                if (manager != null)
                {
                    manager.ManagedStationId = station.Id;
                    await _userManager.UpdateAsync(manager);
                }
            }

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

        await PopulateManagersSelectList(id);
        
        return View(station);
    }

    // POST: FuelStation/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,Address,City,Latitude,Longitude,Phone,Email,IsActive,IsOpen,IsTankerOffloading")] FuelStation station)
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
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Handle manager assignment
                var managerId = Request.Form["ManagerId"].ToString();
                
                // Clear any other managers currently assigned to this station to ensure 1-to-1 mapping via this UI
                var otherManagers = await _userManager.Users.Where(u => u.ManagedStationId == id).ToListAsync();
                foreach (var m in otherManagers)
                {
                    m.ManagedStationId = null;
                    await _userManager.UpdateAsync(m);
                }

                if (!string.IsNullOrEmpty(managerId))
                {
                    var newManager = await _userManager.FindByIdAsync(managerId);
                    if (newManager != null)
                    {
                        // Assign new manager
                        newManager.ManagedStationId = id;
                        await _userManager.UpdateAsync(newManager);
                    }
                }

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
        station.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateManagersSelectList(int? activeStationId = null)
    {
        var managersInRole = await _userManager.GetUsersInRoleAsync(AppRoles.BranchStationManager);
        var managerIds = managersInRole.Select(u => u.Id).ToList();

        // Get full User details including ManagedStation from the business context
        var managerDetails = await _context.Users
            .Include(u => u.ManagedStation)
            .Where(u => managerIds.Contains(u.Id))
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var selectList = managerDetails.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = u.Id,
            Text = u.ManagedStationId.HasValue && u.ManagedStationId != activeStationId
                ? $"{u.UserName} (Already managing: {u.ManagedStation?.Name})"
                : u.UserName,
            Selected = u.ManagedStationId == activeStationId
        }).ToList();

        ViewBag.ManagerId = selectList;
    }

    private async Task<bool> FuelStationExistsAsync(int id)
    {
        return await _context.FuelStations.AnyAsync(e => e.Id == id);
    }

}

