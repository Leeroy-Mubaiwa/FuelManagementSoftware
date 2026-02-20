using System;
using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using FuelManagementSoftware.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize(Roles = AppRoles.OrganisationRoles)]
public class BlockchainTransactionController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly IBlockchainService _blockchainService;
    private readonly ILogger<BlockchainTransactionController> _logger;

    public BlockchainTransactionController(
        FilteredFuelManagementSoftwareDbContext context,
        IBlockchainService blockchainService,
        ILogger<BlockchainTransactionController> logger)
    {
        _context = context;
        _blockchainService = blockchainService;
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
        ViewBag.ExplorerUrl = _blockchainService.IsConfigured() ? _blockchainService.GetExplorerTransactionUrl("") : "";
        ViewBag.ContractAddress = _blockchainService.GetContractAddress();

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

        ViewBag.ExplorerTxUrl = _blockchainService.GetExplorerTransactionUrl(transaction.BlockchainHash);
        ViewBag.ExplorerContractUrl = !string.IsNullOrWhiteSpace(transaction.SmartContractAddress)
            ? _blockchainService.GetExplorerAddressUrl(transaction.SmartContractAddress)
            : "";

        return View(transaction);
    }

    // GET: BlockchainTransaction/Verify
    public IActionResult Verify()
    {
        ViewBag.ContractAddress = _blockchainService.GetContractAddress();
        ViewBag.IsConfigured = _blockchainService.IsConfigured();
        return View();
    }

    // POST: BlockchainTransaction/Verify
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(string transactionNumber)
    {
        ViewBag.ContractAddress = _blockchainService.GetContractAddress();
        ViewBag.IsConfigured = _blockchainService.IsConfigured();

        if (string.IsNullOrWhiteSpace(transactionNumber))
        {
            ViewBag.Error = "Please enter a transaction number.";
            return View();
        }

        // Verify on blockchain
        var chainResult = await _blockchainService.VerifyTransactionOnChainAsync(transactionNumber);

        // Also check local database
        var dbRecord = await _context.BlockchainTransactions
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.FuelStation)
            .Include(bt => bt.FuelTransaction)
            .ThenInclude(ft => ft.FuelType)
            .FirstOrDefaultAsync(bt => bt.FuelTransaction.TransactionNumber == transactionNumber);

        ViewBag.TransactionNumber = transactionNumber;
        ViewBag.ChainResult = chainResult;
        ViewBag.DbRecord = dbRecord;
        ViewBag.ExplorerTxUrl = dbRecord != null
            ? _blockchainService.GetExplorerTransactionUrl(dbRecord.BlockchainHash)
            : "";

        return View();
    }
}
