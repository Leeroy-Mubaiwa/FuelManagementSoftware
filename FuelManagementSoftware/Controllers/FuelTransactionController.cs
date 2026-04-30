using System;
using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize]
public class FuelTransactionController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly ILogger<FuelTransactionController> _logger;

    public FuelTransactionController(
        FilteredFuelManagementSoftwareDbContext context,
        ILogger<FuelTransactionController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: FuelTransaction
    public async Task<IActionResult> Index(int? stationId, string? status, DateTime? startDate, DateTime? endDate)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
        IQueryable<FuelTransaction> query = _context.FuelTransactions
            .Include(t => t.FuelStation)
            .Include(t => t.FuelPump)
            .Include(t => t.FuelType)
            .Include(t => t.PetroCard)
            .Include(t => t.User)
            .AsQueryable();

        if (User.IsInRole(AppRoles.Customer) && user != null)
        {
            query = query.Where(t => t.UserId == user.Id);
        }

        if (stationId.HasValue)
        {
            query = query.Where(t => t.FuelStationId == stationId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.TransactionStatus == status);
        }

        if (startDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= endDate.Value.AddDays(1));
        }

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Take(1000)
            .ToListAsync();

        var stations = await _context.FuelStations
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        ViewBag.Stations = new SelectList(stations, "Id", "Name", stationId);
        ViewBag.Statuses = new SelectList(new[] { "Authorized", "Dispensing", "Completed", "Cancelled", "Failed" }, status);
        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;
        if (stationId.HasValue)
        {
            var station = await _context.FuelStations.FindAsync(stationId.Value);
            ViewBag.StationId = stationId.Value;
            ViewBag.StationName = station?.Name;
        }

        return View(transactions);
    }

    // GET: FuelTransaction/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var transaction = await _context.FuelTransactions
            .Include(t => t.FuelStation)
            .Include(t => t.FuelPump)
            .Include(t => t.FuelType)
            .Include(t => t.PetroCard)
            .Include(t => t.User)
            .Include(t => t.BlockchainTransactions)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
        if (User.IsInRole(AppRoles.Customer) && (user == null || transaction.UserId != user.Id))
        {
            return Forbid();
        }

        ViewBag.StationId = transaction.FuelStationId;
        ViewBag.StationName = transaction.FuelStation?.Name;
        return View(transaction);
    }
}

