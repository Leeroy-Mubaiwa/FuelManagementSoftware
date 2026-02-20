using System;
using System.ComponentModel.DataAnnotations;
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
public class FuelStockController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly IOrganisationContextService _organisationContext;
    private readonly ILogger<FuelStockController> _logger;

    public FuelStockController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        IOrganisationContextService organisationContext,
        ILogger<FuelStockController> logger)
    {
        _context = context;
        _userManager = userManager;
        _organisationContext = organisationContext;
        _logger = logger;
    }

    // GET: FuelStock
    public async Task<IActionResult> Index(int? stationId)
    {
        IQueryable<FuelStock> query = _context.FuelStocks
            .Include(s => s.FuelStation)
            .Include(s => s.FuelType);

        if (stationId.HasValue)
        {
            query = query.Where(s => s.FuelStationId == stationId.Value);
        }

        var stocks = await query.OrderBy(s => s.FuelStation.Name).ThenBy(s => s.FuelType.Name).ToListAsync();
        
        if (stationId.HasValue)
        {
            var station = await _context.FuelStations.FindAsync(stationId.Value);
            ViewBag.StationName = station?.Name;
        }

        return View(stocks);
    }

    // GET: FuelStock/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var stock = await _context.FuelStocks
            .Include(s => s.FuelStation)
            .Include(s => s.FuelType)
            .Include(s => s.Organisation)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stock == null)
        {
            return NotFound();
        }

        return View(stock);
    }

    // GET: FuelStock/Create
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

    // POST: FuelStock/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FuelStationId,FuelTypeId,CurrentQuantity,Capacity,Unit,LowStockThreshold")] FuelStock stock)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var organisationId = await _organisationContext.GetCurrentOrganisationIdAsync();
            if (!organisationId.HasValue) return BadRequest("Organization not found");

            stock.OrganisationId = organisationId.Value;
            stock.CreatorId = user.Id;
            stock.CreatedAt = DateTime.UtcNow;
            stock.LastUpdated = DateTime.UtcNow;
            stock.IsLowStock = stock.LowStockThreshold.HasValue && stock.CurrentQuantity <= stock.LowStockThreshold.Value;

            _context.FuelStocks.Add(stock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { stationId = stock.FuelStationId });
        }

        var stations = await _context.FuelStations
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var fuelTypes = await _context.FuelTypes
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Name)
            .ToListAsync();

        ViewBag.FuelStationId = new SelectList(stations, "Id", "Name", stock.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(fuelTypes, "Id", "Name", stock.FuelTypeId);
        return View(stock);
    }

    // GET: FuelStock/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var stock = await _context.FuelStocks.FindAsync(id);
        if (stock == null)
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

        ViewBag.FuelStationId = new SelectList(stations, "Id", "Name", stock.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(fuelTypes, "Id", "Name", stock.FuelTypeId);
        return View(stock);
    }

    // POST: FuelStock/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,OrganisationId,FuelStationId,FuelTypeId,CurrentQuantity,Capacity,Unit,LowStockThreshold")] FuelStock stock)
    {
        if (id != stock.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                stock.LastUpdated = DateTime.UtcNow;
                stock.IsLowStock = stock.LowStockThreshold.HasValue && stock.CurrentQuantity <= stock.LowStockThreshold.Value;
                _context.Update(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { stationId = stock.FuelStationId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FuelStockExistsAsync(stock.Id))
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

        ViewBag.FuelStationId = new SelectList(stations, "Id", "Name", stock.FuelStationId);
        ViewBag.FuelTypeId = new SelectList(fuelTypes, "Id", "Name", stock.FuelTypeId);
        return View(stock);
    }

    // GET: FuelStock/AddStock/5
    public async Task<IActionResult> AddStock(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var stock = await _context.FuelStocks
            .Include(s => s.FuelStation)
            .Include(s => s.FuelType)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stock == null)
        {
            return NotFound();
        }

        return View(new StockMovementViewModel 
        { 
            FuelStockId = stock.Id, 
            FuelStationId = stock.FuelStationId,
            FuelTypeId = stock.FuelTypeId,
            StationName = stock.FuelStation.Name,
            FuelTypeName = stock.FuelType.Name,
            CurrentQuantity = stock.CurrentQuantity,
            Unit = stock.Unit
        });
    }

    // POST: FuelStock/AddStock/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStock(int id, [Bind("FuelStockId,Quantity,ReferenceNumber,DeliveryNoteNumber,TankerRegistration,DriverName,Notes")] StockMovementViewModel model)
    {
        if (id != model.FuelStockId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var stock = await _context.FuelStocks.FindAsync(id);
                if (stock == null)
                {
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var stockBefore = stock.CurrentQuantity;
                stock.CurrentQuantity += model.Quantity;
                stock.LastUpdated = DateTime.UtcNow;
                stock.IsLowStock = stock.LowStockThreshold.HasValue && stock.CurrentQuantity <= stock.LowStockThreshold.Value;

                var movement = new StockMovement
                {
                    OrganisationId = stock.OrganisationId,
                    FuelStationId = stock.FuelStationId,
                    FuelTypeId = stock.FuelTypeId,
                    MovementType = "Delivery",
                    Quantity = model.Quantity,
                    Unit = stock.Unit,
                    StockBefore = stockBefore,
                    StockAfter = stock.CurrentQuantity,
                    ReferenceNumber = model.ReferenceNumber,
                    DeliveryNoteNumber = model.DeliveryNoteNumber,
                    TankerRegistration = model.TankerRegistration,
                    DriverName = model.DriverName,
                    Notes = model.Notes,
                    MovementDate = DateTime.UtcNow,
                    CreatorId = user.Id
                };

                _context.StockMovements.Add(movement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        var stockForView = await _context.FuelStocks
            .Include(s => s.FuelStation)
            .Include(s => s.FuelType)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stockForView != null)
        {
            model.StationName = stockForView.FuelStation.Name;
            model.FuelTypeName = stockForView.FuelType.Name;
            model.CurrentQuantity = stockForView.CurrentQuantity;
            model.Unit = stockForView.Unit;
        }

        return View(model);
    }

    private async Task<bool> FuelStockExistsAsync(int id)
    {
        return await _context.FuelStocks.AnyAsync(e => e.Id == id);
    }
}

public class StockMovementViewModel
{
    public int FuelStockId { get; set; }
    public int FuelStationId { get; set; }
    public int FuelTypeId { get; set; }
    public string StationName { get; set; } = null!;
    public string FuelTypeName { get; set; } = null!;
    public decimal CurrentQuantity { get; set; }
    public string Unit { get; set; } = null!;
    
    [Required]
    [Display(Name = "Quantity to Add")]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }
    
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }
    
    [Display(Name = "Delivery Note Number")]
    public string? DeliveryNoteNumber { get; set; }
    
    [Display(Name = "Tanker Registration")]
    public string? TankerRegistration { get; set; }
    
    [Display(Name = "Driver Name")]
    public string? DriverName { get; set; }
    
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
}

