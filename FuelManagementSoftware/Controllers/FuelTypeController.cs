using System;
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

    // GET: FuelType/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var fuelType = await _context.FuelTypes.FindAsync(id);
        if (fuelType == null) return NotFound();

        return View(fuelType);
    }

    // POST: FuelType/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,Description,UnitPrice,Unit,IsActive")] FuelType fuelType)
    {
        if (id != fuelType.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(fuelType);
        }

        var existing = await _context.FuelTypes.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = fuelType.Name;
        existing.Code = fuelType.Code;
        existing.Description = fuelType.Description;
        existing.UnitPrice = fuelType.UnitPrice;
        existing.Unit = fuelType.Unit;
        existing.IsActive = fuelType.IsActive;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

