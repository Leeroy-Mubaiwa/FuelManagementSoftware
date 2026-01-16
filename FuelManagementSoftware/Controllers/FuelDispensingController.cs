using System;
using System.Linq;
using System.Threading.Tasks;
using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using FuelManagementSoftware.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize]
public class FuelDispensingController : Controller
{
    private readonly IFuelDispensingService _fuelDispensingService;
    private readonly INfcReaderService _nfcReaderService;
    private readonly IPetroCardService _petroCardService;
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly ILogger<FuelDispensingController> _logger;

    public FuelDispensingController(
        IFuelDispensingService fuelDispensingService,
        INfcReaderService nfcReaderService,
        IPetroCardService petroCardService,
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        ILogger<FuelDispensingController> logger)
    {
        _fuelDispensingService = fuelDispensingService;
        _nfcReaderService = nfcReaderService;
        _petroCardService = petroCardService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: FuelDispensing/Index
    public async Task<IActionResult> Index()
    {
        var stations = await _context.FuelStations
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        ViewBag.Stations = new SelectList(stations, "Id", "Name");
        return View();
    }

    // GET: FuelDispensing/SelectPump?stationId=1
    public async Task<IActionResult> SelectPump(int stationId)
    {
        var station = await _context.FuelStations
            .Include(s => s.FuelPumps)
            .ThenInclude(p => p.FuelType)
            .FirstOrDefaultAsync(s => s.Id == stationId);

        if (station == null)
        {
            return NotFound();
        }

        if (!station.IsOpen || station.IsTankerOffloading)
        {
            ViewBag.ErrorMessage = station.IsTankerOffloading 
                ? "Station is currently closed for tanker offloading." 
                : "Station is currently closed.";
            return View("Index");
        }

        ViewBag.Station = station;
        return View(station.FuelPumps.Where(p => p.IsActive && p.IsOperational).ToList());
    }

    // GET: FuelDispensing/StartDispensing?pumpId=1
    public async Task<IActionResult> StartDispensing(int pumpId)
    {
        var pump = await _context.FuelPumps
            .Include(p => p.FuelStation)
            .Include(p => p.FuelType)
            .FirstOrDefaultAsync(p => p.Id == pumpId);

        if (pump == null)
        {
            return NotFound();
        }

        ViewBag.Pump = pump;
        return View();
    }

    // POST: FuelDispensing/Initiate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Initiate([FromForm] DispensingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            request.CreatorId = user.Id;

            var transaction = await _fuelDispensingService.InitiateDispensingAsync(request);
            
            return RedirectToAction(nameof(Dispensing), new { transactionId = transaction.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating fuel dispensing");
            ModelState.AddModelError("", ex.Message);
            return View("StartDispensing", new { pumpId = request.FuelPumpId });
        }
    }

    // GET: FuelDispensing/Dispensing?transactionId=1
    public async Task<IActionResult> Dispensing(int transactionId)
    {
        var transaction = await _context.FuelTransactions
            .Include(t => t.FuelPump)
            .Include(t => t.FuelStation)
            .Include(t => t.FuelType)
            .Include(t => t.PetroCard)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction == null)
        {
            return NotFound();
        }

        return View(transaction);
    }

    // POST: FuelDispensing/Start
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(int transactionId)
    {
        try
        {
            var transaction = await _fuelDispensingService.StartDispensingAsync(transactionId);
            return Json(new { success = true, transactionId = transaction.Id, status = transaction.TransactionStatus });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting fuel dispensing");
            return Json(new { success = false, error = ex.Message });
        }
    }

    // POST: FuelDispensing/Complete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int transactionId, decimal finalQuantity)
    {
        try
        {
            var transaction = await _fuelDispensingService.CompleteDispensingAsync(transactionId, finalQuantity);
            return Json(new { success = true, transactionId = transaction.Id, status = transaction.TransactionStatus });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing fuel dispensing");
            return Json(new { success = false, error = ex.Message });
        }
    }

    // POST: FuelDispensing/Cancel
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int transactionId, string? reason = null)
    {
        try
        {
            var transaction = await _fuelDispensingService.CancelDispensingAsync(transactionId, reason);
            return Json(new { success = true, transactionId = transaction.Id, status = transaction.TransactionStatus });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling fuel dispensing");
            return Json(new { success = false, error = ex.Message });
        }
    }

    // GET: FuelDispensing/ReadNfcTag?pumpId=1
    [HttpGet]
    public async Task<IActionResult> ReadNfcTag(int pumpId)
    {
        try
        {
            var nfcTag = await _nfcReaderService.WaitForNfcTagAsync(pumpId, timeoutMs: 10000);
            
            if (string.IsNullOrEmpty(nfcTag))
            {
                return Json(new { success = false, message = "No NFC tag detected" });
            }

            var card = await _petroCardService.GetCardByNfcTagAsync(nfcTag);
            
            if (card == null)
            {
                return Json(new { success = false, message = "Card not found" });
            }

            return Json(new 
            { 
                success = true, 
                nfcTag = nfcTag,
                cardId = card.Id,
                cardNumber = card.CardNumber,
                balance = card.Balance,
                currency = card.Currency,
                requiresPin = !string.IsNullOrWhiteSpace(card.PinHash)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading NFC tag");
            return Json(new { success = false, error = ex.Message });
        }
    }
}

