using System;
using System.ComponentModel.DataAnnotations;
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
public class PetroCardController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly IPetroCardService _petroCardService;
    private readonly ILogger<PetroCardController> _logger;

    public PetroCardController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        IPetroCardService petroCardService,
        ILogger<PetroCardController> logger)
    {
        _context = context;
        _userManager = userManager;
        _petroCardService = petroCardService;
        _logger = logger;
    }

    // GET: PetroCard
    public async Task<IActionResult> Index()
    {
        var cards = await _context.PetroCards
            .Include(c => c.User)
            .Include(c => c.Organisation)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return View(cards);
    }

    // GET: PetroCard/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var card = await _context.PetroCards
            .Include(c => c.User)
            .Include(c => c.Organisation)
            .Include(c => c.CardTransactions.OrderByDescending(t => t.TransactionDate).Take(10))
            .Include(c => c.FuelTransactions.OrderByDescending(t => t.TransactionDate).Take(10))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null)
        {
            return NotFound();
        }

        return View(card);
    }

    // GET: PetroCard/Create
    public async Task<IActionResult> Create()
    {
        var users = await _context.Users
            .OrderBy(u => u.UserName)
            .Select(u => new { u.Id, u.UserName })
            .ToListAsync();

        ViewBag.UserId = new SelectList(users, "Id", "UserName");
        return View();
    }

    // POST: PetroCard/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CardNumber,Rfidtag,UserId,Balance,Currency,IsActive,IsBlocked,ExpiryDate,Pin")] PetroCardViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var organisationId = await GetCurrentOrganisationIdAsync();
            if (!organisationId.HasValue) return BadRequest("Organization not found");

            var card = new PetroCard
            {
                OrganisationId = organisationId.Value,
                CardNumber = model.CardNumber,
                Rfidtag = model.Rfidtag,
                UserId = model.UserId,
                Balance = model.Balance,
                Currency = model.Currency ?? "USD",
                IsActive = model.IsActive,
                IsBlocked = model.IsBlocked,
                ExpiryDate = model.ExpiryDate,
                PinHash = !string.IsNullOrWhiteSpace(model.Pin) ? PetroCardService.HashPin(model.Pin) : null,
                CreatedAt = DateTime.UtcNow,
                CreatorId = user.Id
            };

            _context.PetroCards.Add(card);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var users = await _context.Users
            .OrderBy(u => u.UserName)
            .Select(u => new { u.Id, u.UserName })
            .ToListAsync();

        ViewBag.UserId = new SelectList(users, "Id", "UserName");
        return View(model);
    }

    // GET: PetroCard/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var card = await _context.PetroCards.FindAsync(id);
        if (card == null)
        {
            return NotFound();
        }

        var model = new PetroCardViewModel
        {
            Id = card.Id,
            CardNumber = card.CardNumber,
            Rfidtag = card.Rfidtag,
            UserId = card.UserId,
            Balance = card.Balance,
            Currency = card.Currency,
            IsActive = card.IsActive,
            IsBlocked = card.IsBlocked,
            ExpiryDate = card.ExpiryDate
        };

        var users = await _context.Users
            .OrderBy(u => u.UserName)
            .Select(u => new { u.Id, u.UserName })
            .ToListAsync();

        ViewBag.UserId = new SelectList(users, "Id", "UserName");
        return View(model);
    }

    // POST: PetroCard/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CardNumber,Rfidtag,UserId,Balance,Currency,IsActive,IsBlocked,ExpiryDate,Pin")] PetroCardViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var card = await _context.PetroCards.FindAsync(id);
                if (card == null)
                {
                    return NotFound();
                }

                card.CardNumber = model.CardNumber;
                card.Rfidtag = model.Rfidtag;
                card.UserId = model.UserId;
                card.Balance = model.Balance;
                card.Currency = model.Currency ?? "USD";
                card.IsActive = model.IsActive;
                card.IsBlocked = model.IsBlocked;
                card.ExpiryDate = model.ExpiryDate;
                card.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(model.Pin))
                {
                    card.PinHash = PetroCardService.HashPin(model.Pin);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PetroCardExistsAsync(model.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        var users = await _context.Users
            .OrderBy(u => u.UserName)
            .Select(u => new { u.Id, u.UserName })
            .ToListAsync();

        ViewBag.UserId = new SelectList(users, "Id", "UserName");
        return View(model);
    }

    // GET: PetroCard/TopUp/5
    public async Task<IActionResult> TopUp(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var card = await _context.PetroCards
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null)
        {
            return NotFound();
        }

        return View(new TopUpViewModel { CardId = card.Id, CardNumber = card.CardNumber, CurrentBalance = card.Balance, Currency = card.Currency });
    }

    // POST: PetroCard/TopUp/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopUp(int id, [Bind("CardId,Amount,PaymentMethod,ReferenceNumber")] TopUpViewModel model)
    {
        if (id != model.CardId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                await _petroCardService.DeductBalanceAsync(id, -model.Amount, "TopUp", model.ReferenceNumber, user.Id);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        var card = await _context.PetroCards
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card != null)
        {
            model.CardNumber = card.CardNumber;
            model.CurrentBalance = card.Balance;
            model.Currency = card.Currency;
        }

        return View(model);
    }

    private async Task<bool> PetroCardExistsAsync(int id)
    {
        return await _context.PetroCards.AnyAsync(e => e.Id == id);
    }

    private async Task<int?> GetCurrentOrganisationIdAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return null;

        var card = await _context.PetroCards
            .Where(c => c.UserId == user.Id && c.IsActive)
            .OrderByDescending(c => c.LastUsedAt ?? c.CreatedAt)
            .FirstOrDefaultAsync();

        return card?.OrganisationId;
    }
}

public class PetroCardViewModel
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Card Number")]
    public string CardNumber { get; set; } = null!;
    
    [Display(Name = "NFC Tag")]
    public string? Rfidtag { get; set; }
    
    [Required]
    [Display(Name = "User")]
    public string UserId { get; set; } = null!;
    
    [Required]
    [Display(Name = "Balance")]
    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; }
    
    [Display(Name = "Currency")]
    public string Currency { get; set; } = "USD";
    
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Blocked")]
    public bool IsBlocked { get; set; }
    
    [Display(Name = "Expiry Date")]
    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }
    
    [Display(Name = "PIN (leave blank to keep current)")]
    [StringLength(6, MinimumLength = 4)]
    public string? Pin { get; set; }
}

public class TopUpViewModel
{
    public int CardId { get; set; }
    public string CardNumber { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public string Currency { get; set; } = "USD";
    
    [Required]
    [Display(Name = "Top-Up Amount")]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [Display(Name = "Payment Method")]
    public string? PaymentMethod { get; set; }
    
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }
}

