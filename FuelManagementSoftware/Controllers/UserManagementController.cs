using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuelManagementSoftware.Controllers
{
    [Authorize(Roles = AppRoles.PetrotradeAdmin)]
    public class UserManagementController : Controller
    {
        private readonly UserManager<FuelManagementSoftwareUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly FuelManagementSoftwareDbContext _context;

        public UserManagementController(
            UserManager<FuelManagementSoftwareUser> userManager,
            RoleManager<IdentityRole> roleManager,
            FuelManagementSoftwareDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(user.Id, roles);
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.Stations = await _context.FuelStations.ToDictionaryAsync(s => s.Id, s => s.Name);
            
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.AllRoles = allRoles;
            ViewBag.UserRoles = userRoles;
            ViewBag.Stations = new SelectList(await _context.FuelStations.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", user.ManagedStationId);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int? managedStationId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.ManagedStationId = managedStationId;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user properties.");
                return await Edit(id);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            return RedirectToAction(nameof(Index));
        }
    }
}
