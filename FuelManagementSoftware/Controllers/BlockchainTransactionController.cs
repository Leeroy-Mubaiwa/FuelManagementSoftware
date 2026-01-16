using System;
using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize]
public class BlockchainTransactionController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly ILogger<BlockchainTransactionController> _logger;

    public BlockchainTransactionController(
        FilteredFuelManagementSoftwareDbContext context,
        ILogger<BlockchainTransactionController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: BlockchainTransaction
    public async Task<IActionResult> Index(string? status, DateTime? startDate, DateTime? endDate)
    {
        IQueryable<BlockchainTransaction> query = _context.BlockchainTransactions
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.FuelStation)
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.PetroCard)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(bt => bt.Status == status);
        }

        if (startDate.HasValue)
        {
            query = query.Where(bt => bt.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(bt => bt.CreatedAt <= endDate.Value.AddDays(1));
        }

        var transactions = await query
            .OrderByDescending(bt => bt.CreatedAt)
            .Take(1000)
            .ToListAsync();

        ViewBag.Statuses = new SelectList(new[] { "Pending", "Confirmed", "Failed" }, status);
        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;

        return View(transactions);
    }

    // GET: BlockchainTransaction/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var transaction = await _context.BlockchainTransactions
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.FuelStation)
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.FuelPump)
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.FuelType)
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.PetroCard)
            .FirstOrDefaultAsync(bt => bt.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        return View(transaction);
    }
}

