// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using FuelManagementSoftware.Areas.Identity.Data;
using FuelManagementSoftware.Constants;
using FuelManagementSoftware.Data;
using FuelManagementSoftware.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FuelManagementSoftware.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<FuelManagementSoftwareUser> _signInManager;
        private readonly UserManager<FuelManagementSoftwareUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<FuelManagementSoftwareUser> _userStore;
        private readonly IUserEmailStore<FuelManagementSoftwareUser> _emailStore;
        private readonly FuelManagementSoftwareDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<FuelManagementSoftwareUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<FuelManagementSoftwareUser> userStore,
            SignInManager<FuelManagementSoftwareUser> signInManager,
            FuelManagementSoftwareDbContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    var userId = await _userManager.GetUserIdAsync(user);

                    await AssignRoleAsync(user, AppRoles.Customer);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private FuelManagementSoftwareUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<FuelManagementSoftwareUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(FuelManagementSoftwareUser)}'. " +
                    $"Ensure that '{nameof(FuelManagementSoftwareUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<FuelManagementSoftwareUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<FuelManagementSoftwareUser>)_userStore;
        }

        /// <summary>
        /// Assigns the specified role to the user.
        /// </summary>
        private async Task AssignRoleAsync(FuelManagementSoftwareUser user, string roleName)
        {
            // Check if role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning("Role '{RoleName}' does not exist. Creating it now.", roleName);
                var role = new IdentityRole(roleName)
                {
                    NormalizedName = roleName.ToUpperInvariant()
                };
                await _roleManager.CreateAsync(role);
            }

            // Assign role to user
            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (roleResult.Succeeded)
            {
                _logger.LogInformation("User {UserId} assigned to role {RoleName}", user.Id, roleName);
            }
            else
            {
                _logger.LogError("Failed to assign role {RoleName} to user {UserId}: {Errors}", 
                    roleName, user.Id, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
