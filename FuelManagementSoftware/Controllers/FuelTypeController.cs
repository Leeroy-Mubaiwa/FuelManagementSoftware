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
public class FuelTypeController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly IOrganisationContextService _organisationContext;
    private readonly ILogger<FuelTypeController> _logger;

    public FuelTypeController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        IOrganisationContextService organisationContext,
        ILogger<FuelTypeController> logger)
    {
        _context = context;
        _userManager = userManager;
        _organisationContext = organisationContext;
        _logger = logger;
    }

    // GET: FuelType
    public async Task<IActionResult> Index()
    {
        var fuelTypes = await _context.FuelTypes
            .Include(ft => ft.Organisation)
            .OrderBy(ft => ft.Name)
            .ToListAsync();
        return View(fuelTypes);
    }

    // GET: FuelType/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fuelType = await _context.FuelTypes
            .Include(ft => ft.Organisation)
            .FirstOrDefaultAsync(ft => ft.Id == id);

        if (fuelType == null)
        {
            return NotFound();
        }

        return View(fuelType);
    }

    // GET: FuelType/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: FuelType/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Code,Description,UnitPrice,Unit,IsActive")] FuelType fuelType)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var organisationId = await _organisationContext.GetCurrentOrganisationIdAsync();
            if (!organisationId.HasValue) return BadRequest("Organization not found");

            fuelType.OrganisationId = organisationId.Value;
            fuelType.CreatorId = user.Id;
            fuelType.CreatedAt = DateTime.UtcNow;

            _context.FuelTypes.Add(fuelType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(fuelType);
    }

    // GET: FuelType/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fuelType = await _context.FuelTypes.FindAsync(id);
        if (fuelType == null)
        {
            return NotFound();
        }
        return View(fuelType);
    }

    // POST: FuelType/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,OrganisationId,Name,Code,Description,UnitPrice,Unit,IsActive")] FuelType fuelType)
    {
        if (id != fuelType.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                fuelType.UpdatedAt = DateTime.UtcNow;
                _context.Update(fuelType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FuelTypeExistsAsync(fuelType.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        return View(fuelType);
    }

    private async Task<bool> FuelTypeExistsAsync(int id)
    {
        return await _context.FuelTypes.AnyAsync(e => e.Id == id);
    }
}

