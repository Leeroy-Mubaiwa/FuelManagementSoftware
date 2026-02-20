using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSoftware.Controllers;

[Authorize]
public class FuelAvailabilityController : Controller
{
    private const string OsrmBase = "https://router.project-osrm.org";
    private readonly FuelManagementSoftwareDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FuelAvailabilityController> _logger;

    public FuelAvailabilityController(
        FuelManagementSoftwareDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<FuelAvailabilityController> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // GET: FuelAvailability (uses unfiltered context so Customers see all stations)
    public async Task<IActionResult> Index(int? fuelTypeId, decimal? latitude, decimal? longitude)
    {
        IQueryable<FuelStation> query = _context.FuelStations
            .Include(s => s.FuelStocks)
            .ThenInclude(st => st.FuelType)
            .Include(s => s.FuelPumps.Where(p => p.IsActive && p.IsOperational))
            .Where(s => s.IsActive && s.IsOpen && !s.IsTankerOffloading);

        var stations = await query.ToListAsync();

        // Filter by fuel type if specified
        if (fuelTypeId.HasValue)
        {
            stations = stations.Where(s => s.FuelStocks.Any(st => st.FuelTypeId == fuelTypeId.Value && st.CurrentQuantity > 0)).ToList();
        }

        // Calculate distances if coordinates provided
        var stationsWithDistance = stations.Select(s => new
        {
            Station = s,
            Distance = (s.Latitude.HasValue && s.Longitude.HasValue && latitude.HasValue && longitude.HasValue)
                ? CalculateDistance(latitude.Value, longitude.Value, s.Latitude.Value, s.Longitude.Value)
                : (double?)null
        }).OrderBy(x => x.Distance ?? double.MaxValue).Select(x => x.Station).ToList();

        stations = stationsWithDistance;

        var fuelTypes = await _context.FuelTypes
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Name)
            .ToListAsync();

        ViewBag.FuelTypes = new SelectList(fuelTypes, "Id", "Name", fuelTypeId);
        ViewBag.Latitude = latitude;
        ViewBag.Longitude = longitude;

        return View(stations);
    }

    // GET: FuelAvailability/Map
    public async Task<IActionResult> Map(int? stationId, int? fuelTypeId)
    {
        IQueryable<FuelStation> query = _context.FuelStations
            .Include(s => s.FuelStocks)
            .ThenInclude(st => st.FuelType)
            .Include(s => s.FuelPumps)
            .Where(s => s.IsActive && s.Latitude.HasValue && s.Longitude.HasValue);

        if (fuelTypeId.HasValue)
        {
            query = query.Where(s => s.FuelStocks.Any(st => st.FuelTypeId == fuelTypeId.Value && st.CurrentQuantity > 0));
        }

        var stations = await query.ToListAsync();

        var fuelTypes = await _context.FuelTypes
            .Where(ft => ft.IsActive)
            .OrderBy(ft => ft.Name)
            .ToListAsync();

        ViewBag.FuelTypes = new SelectList(fuelTypes, "Id", "Name", fuelTypeId);
        ViewBag.SelectedStationId = stationId;

        return View(stations);
    }

    // GET: FuelAvailability/StationDetails/5
    public async Task<IActionResult> StationDetails(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var station = await _context.FuelStations
            .Include(s => s.FuelStocks)
            .ThenInclude(st => st.FuelType)
            .Include(s => s.FuelPumps)
            .ThenInclude(p => p.FuelType)
            .Include(s => s.QueueInformations.OrderByDescending(q => q.RecordedAt).Take(5))
            .FirstOrDefaultAsync(s => s.Id == id);

        if (station == null)
        {
            return NotFound();
        }

        return View(station);
    }

    // GET: FuelAvailability/OsrmProxy/route/v1/{**path} — proxy for OSRM to avoid CORS
    [HttpGet("OsrmProxy/route/v1/{**path}")]
    public async Task<IActionResult> OsrmProxy(string path)
    {
        var query = Request.QueryString.HasValue ? Request.QueryString.Value : "";
        var url = $"{OsrmBase}/route/v1/{path}{query}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OSRM proxy failed for {Url}", url);
            return StatusCode(502, "Routing service temporarily unavailable.");
        }
    }

    // API: GET: FuelAvailability/GetAvailability
    [HttpGet]
    public async Task<IActionResult> GetAvailability(int? stationId, int? fuelTypeId)
    {
        IQueryable<FuelStock> query = _context.FuelStocks
            .Include(st => st.FuelStation)
            .Include(st => st.FuelType);

        if (stationId.HasValue)
        {
            query = query.Where(st => st.FuelStationId == stationId.Value);
        }

        if (fuelTypeId.HasValue)
        {
            query = query.Where(st => st.FuelTypeId == fuelTypeId.Value);
        }

        var stocks = await query
            .Where(st => st.FuelStation.IsActive && st.FuelStation.IsOpen && !st.FuelStation.IsTankerOffloading)
            .Select(st => new
            {
                StationId = st.FuelStationId,
                StationName = st.FuelStation.Name,
                StationAddress = st.FuelStation.Address,
                StationLatitude = st.FuelStation.Latitude,
                StationLongitude = st.FuelStation.Longitude,
                StationIsOpen = st.FuelStation.IsOpen,
                StationIsTankerOffloading = st.FuelStation.IsTankerOffloading,
                FuelTypeId = st.FuelTypeId,
                FuelTypeName = st.FuelType.Name,
                CurrentQuantity = st.CurrentQuantity,
                Capacity = st.Capacity,
                IsLowStock = st.IsLowStock,
                LastUpdated = st.LastUpdated
            })
            .ToListAsync();

        return Json(stocks);
    }

    private double? CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        // Haversine formula to calculate distance in kilometers
        const double R = 6371; // Earth's radius in kilometers
        var dLat = (double)(lat2 - lat1) * Math.PI / 180;
        var dLon = (double)(lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos((double)lat1 * Math.PI / 180) * Math.Cos((double)lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}


