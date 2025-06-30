using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MiniAccountManagement.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MiniAccountManagement.Pages.Admin
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<CreateUserModel> _logger;

        [BindProperty]
        public CreateUserViewModel Input { get; set; }

        public CreateUserModel(UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, ILogger<CreateUserModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, Input.Email, System.Threading.CancellationToken.None);
            await _userManager.SetEmailAsync(user, Input.Email);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Input.SelectedRole);
                _logger.LogInformation($"User '{user.Email}' created and assigned to '{Input.SelectedRole}' role.");

                // If the current user is an Admin, auto-confirm/approve the new account.
                if (User.IsInRole("Admin"))
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, code);
                    _logger.LogInformation($"User '{user.Email}' was auto-approved by Admin.");
                }
                else
                {
                    // If created by an Accountant, it remains pending for admin approval.
                    _logger.LogInformation($"User '{user.Email}' created by Accountant. Awaiting admin approval.");
                }

                return RedirectToPage("./UserManagement");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private IdentityUser CreateUser()
        {
            try { return Activator.CreateInstance<IdentityUser>(); }
            catch { throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'."); }
        }
    }
}