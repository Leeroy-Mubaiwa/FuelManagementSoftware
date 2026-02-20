using System.Diagnostics;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using FuelManagementSoftware.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FilteredFuelManagementSoftwareDbContext _context;
        private readonly IOrganisationContextService _orgContext;

        public HomeController(
            ILogger<HomeController> logger,
            FilteredFuelManagementSoftwareDbContext context,
            IOrganisationContextService orgContext)
        {
            _logger = logger;
            _context = context;
            _orgContext = orgContext;
        }

        public async Task<IActionResult> Index()
        {
            var orgId = await _orgContext.GetCurrentOrganisationIdAsync();
            if (orgId.HasValue)
            {
                ViewBag.StationsCount = await _context.FuelStations.CountAsync();
                ViewBag.PumpsCount = await _context.FuelPumps.CountAsync();
                var completed = _context.FuelTransactions.Where(t => t.TransactionStatus == "Completed");
                ViewBag.TotalSales = (decimal)await completed.SumAsync(t => (double)t.TotalAmount);
                ViewBag.TotalFuelLitres = (decimal)await completed.SumAsync(t => (double)t.Quantity);
                ViewBag.TransactionsCount = await completed.CountAsync();
                ViewBag.RecentTransactions = await _context.FuelTransactions
                    .Include(t => t.FuelStation)
                    .Include(t => t.FuelType)
                    .Where(t => t.TransactionStatus == "Completed")
                    .OrderByDescending(t => t.CompletedAt)
                    .Take(10)
                    .ToListAsync();
                var start = DateTime.Now.Date.AddDays(-13);
                var salesByDay = await _context.FuelTransactions
                    .Where(t => t.TransactionStatus == "Completed" && t.CompletedAt.HasValue && t.CompletedAt >= start)
                    .GroupBy(t => t.CompletedAt.Value.Date)
                    .Select(g => new { Date = g.Key, Total = g.Sum(t => (double)t.TotalAmount), Litres = g.Sum(t => (double)t.Quantity) })
                    .OrderBy(x => x.Date)
                    .ToListAsync();
                ViewBag.SalesByDay = salesByDay;
                ViewBag.SalesChartLabels = salesByDay.Select(x => x.Date.ToString("MMM d")).ToList();
                ViewBag.SalesChartData = salesByDay.Select(x => (double)x.Total).ToList();
                var lowStock = await _context.FuelStocks
                    .Include(s => s.FuelStation)
                    .Include(s => s.FuelType)
                    .Where(s => s.IsLowStock || s.CurrentQuantity <= 0)
                    .Take(5)
                    .ToListAsync();
                ViewBag.LowStock = lowStock;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Manual()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
