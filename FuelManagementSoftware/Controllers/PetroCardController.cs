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
using Webdev.Payments; 

namespace FuelManagementSoftware.Controllers;

[Authorize]
public class PetroCardController : Controller
{
    private readonly FilteredFuelManagementSoftwareDbContext _context;
    private readonly UserManager<FuelManagementSoftwareUser> _userManager;
    private readonly IPetroCardService _petroCardService;
    private readonly ILogger<PetroCardController> _logger;
    private readonly IConfiguration _configuration;

    public PetroCardController(
        FilteredFuelManagementSoftwareDbContext context,
        UserManager<FuelManagementSoftwareUser> userManager,
        IPetroCardService petroCardService,
        ILogger<PetroCardController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _petroCardService = petroCardService;
        _logger = logger;
        _configuration = configuration;
    }

    // GET: PetroCard
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var query = _context.PetroCards
            .Include(c => c.User)
            .Include(c => c.Organisation)
            .AsQueryable();

        if (User.IsInRole(AppRoles.Customer))
        {
            query = query.Where(c => c.UserId == user.Id);
        }

        var cards = await query
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

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

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

        // Security check for customers
        if (User.IsInRole(AppRoles.Customer) && card.UserId != user.Id)
        {
            return Forbid();
        }

        return View(card);
    }

    // GET: PetroCard/Create
    [Authorize(Roles = AppRoles.OrganisationRoles)]
    public IActionResult Create()
    {
        return View();
    }

    // POST: PetroCard/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.OrganisationRoles)]
    public async Task<IActionResult> Create([Bind("CardNumber,Rfidtag,Balance,Currency,IsActive,IsBlocked,ExpiryDate,Pin")] PetroCardViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var card = new PetroCard
            {
                CardNumber = model.CardNumber,
                Rfidtag = model.Rfidtag,
                UserId = user.Id,
                Balance = model.Balance,
                Currency = model.Currency ?? "USD",
                IsActive = model.IsActive,
                IsBlocked = model.IsBlocked,
                ExpiryDate = model.ExpiryDate,
                PinHash = !string.IsNullOrWhiteSpace(model.Pin) ? PetroCardService.HashPin(model.Pin) : null,
                CreatedAt = DateTime.Now,
                CreatorId = user.Id
            };

            _context.PetroCards.Add(card);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // GET: PetroCard/Edit/5
    [Authorize(Roles = AppRoles.OrganisationRoles)]
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
    [Authorize(Roles = AppRoles.OrganisationRoles)]
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
                card.UpdatedAt = DateTime.Now;

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

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Security check for customers
        if (User.IsInRole(AppRoles.Customer) && card.UserId != user.Id)
        {
            return Forbid();
        }

        return View(new TopUpViewModel { CardId = card.Id, CardNumber = card.CardNumber, CurrentBalance = card.Balance, Currency = card.Currency });
    }

    // POST: PetroCard/TopUp/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopUp(int id, [Bind("CardId,Amount,PaymentMethod,EcocashPhoneNumber,ReferenceNumber")] TopUpViewModel model)
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

                var card = await _context.PetroCards.FindAsync(id);
                if (card == null) return NotFound();

                // Security check for customers
                if (User.IsInRole(AppRoles.Customer) && card.UserId != user.Id)
                {
                    return Forbid();
                }

                // Initialise Paynow
                var integrationId = _configuration["Paynow:IntegrationId"];
                var integrationKey = _configuration["Paynow:IntegrationKey"];
                var resultUrl = _configuration["Paynow:ResultUrl"];
                var returnUrl = _configuration["Paynow:ReturnUrl"];

                if (string.IsNullOrEmpty(integrationId) || string.IsNullOrEmpty(integrationKey))
                {
                    ModelState.AddModelError("", "Payment gateway is not configured. Please contact support.");
                    return await ReloadTopUpView(id, model);
                }

                var paynow = new Paynow(integrationId, integrationKey);
                if (!string.IsNullOrEmpty(resultUrl))
                    paynow.ResultUrl = resultUrl;
                if (!string.IsNullOrEmpty(returnUrl))
                    paynow.ReturnUrl = returnUrl;

                // Build reference
                var internalRef = $"TOPUP-{model.CardId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                // Create mobile payment for Ecocash
                var payment = paynow.CreatePayment(internalRef, user.Email ?? user.UserName ?? "customer@petrochain.co.zw");
                payment.Add($"PetroCard Top-Up ({model.Amount:F2} {model.Currency})", (decimal)model.Amount);

                // Send via Ecocash mobile
                var phone = model.EcocashPhoneNumber?.Trim() ?? "";
                if (string.IsNullOrEmpty(phone))
                {
                    ModelState.AddModelError("EcocashPhoneNumber", "Ecocash phone number is required.");
                    return await ReloadTopUpView(id, model);
                }

                var response = await paynow.SendMobileAsync(payment, phone,"ecocash");

                if (!response.Success())
                {
                    var errorMsg = "Ecocash payment initiation failed. Please check your phone number and try again.";
                    try { errorMsg = response.Errors(); } catch { }
                    ModelState.AddModelError("", errorMsg);
                    return await ReloadTopUpView(id, model);
                }

                // Payment sent – store poll URL so we can check status
                var pollUrl = response.PollUrl();
                string? instructions = null;
                try { 
                    instructions = response.ToString(); 
                } catch { }

                // Redirect to status page where the user waits for approval
                TempData["PollUrl"] = pollUrl;
                TempData["TopUpAmount"] = model.Amount.ToString("F2");
                TempData["TopUpCardId"] = model.CardId.ToString();
                TempData["TopUpRef"] = internalRef;
                TempData["TopUpInstructions"] = instructions ?? "Approve the payment on your Ecocash phone to complete the top-up.";

                return RedirectToAction(nameof(TopUpStatus), new { id = model.CardId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating Ecocash top-up for card {CardId}", model.CardId);
                ModelState.AddModelError("", "An error occurred while processing your payment. Please try again.");
            }
        }

        return await ReloadTopUpView(id, model);
    }

    // GET: PetroCard/TopUpStatus/5
    public IActionResult TopUpStatus(int id)
    {
        var pollUrl = TempData["PollUrl"]?.ToString();
        var amount = TempData["TopUpAmount"]?.ToString();
        var cardId = TempData["TopUpCardId"]?.ToString();
        var reference = TempData["TopUpRef"]?.ToString();
        var instructions = TempData["TopUpInstructions"]?.ToString();

        if (string.IsNullOrEmpty(pollUrl))
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        ViewBag.PollUrl = pollUrl;
        ViewBag.Amount = amount;
        ViewBag.CardId = cardId;
        ViewBag.Reference = reference;
        ViewBag.Instructions = instructions;
        return View();
    }

    // POST: PetroCard/CheckPaymentStatus (AJAX)
    [HttpPost]
    public async Task<IActionResult> CheckPaymentStatus([FromBody] CheckPaymentRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.PollUrl))
        {
            return Json(new { success = false, message = "Invalid request" });
        }

        try
        {
            var integrationId = _configuration["Paynow:IntegrationId"];
            var integrationKey = _configuration["Paynow:IntegrationKey"];
            var paynow = new Paynow(integrationId!, integrationKey!);

            var status = paynow.PollTransaction(request.PollUrl);

            if (status.Paid())
            {
                // Credit the card
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Json(new { success = false, message = "Unauthorized" });

                var amount = decimal.Parse(request.Amount ?? "0");
                var cardId = int.Parse(request.CardId ?? "0");

                if (amount > 0 && cardId > 0)
                {
                    await _petroCardService.DeductBalanceAsync(
                        cardId, -amount, "TopUp",
                        request.Reference ?? "Ecocash",
                        user.Id);
                }

                return Json(new { success = true, paid = true, message = "Payment confirmed! Your card has been topped up." });
            }

            // Not yet paid
            return Json(new { success = true, paid = false, message = "Awaiting payment confirmation..." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Ecocash payment status");
            return Json(new { success = false, message = "Error checking payment status." });
        }
    }

    private async Task<IActionResult> ReloadTopUpView(int id, TopUpViewModel model)
    {
        var card = await _context.PetroCards
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card != null)
        {
            model.CardNumber = card.CardNumber;
            model.CurrentBalance = card.Balance;
            model.Currency = card.Currency;
        }

        return View("TopUp", model);
    }

    public class CheckPaymentRequest
    {
        public string? PollUrl { get; set; }
        public string? Amount { get; set; }
        public string? CardId { get; set; }
        public string? Reference { get; set; }
    }

    private async Task<bool> PetroCardExistsAsync(int id)
    {
        return await _context.PetroCards.AnyAsync(e => e.Id == id);
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
    
    [Display(Name = "User")]
    public string? UserId { get; set; }
    
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
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    
    [Display(Name = "Payment Method")]
    public string? PaymentMethod { get; set; }

    [Required(ErrorMessage = "Ecocash phone number is required.")]
    [Display(Name = "Ecocash Phone Number")]
    [RegularExpression(@"^(077|078)\d{7}$", ErrorMessage = "Enter a valid Ecocash number (e.g. 0771234567).")]
    public string? EcocashPhoneNumber { get; set; }
    
    [Display(Name = "Reference Number")]
    public string? ReferenceNumber { get; set; }
}

