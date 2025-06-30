using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniAccountManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniAccountManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserManagementModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public List<UserWithRolesViewModel> UsersWithRoles { get; set; }

        public UserManagementModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task OnGetAsync()
        {
            // Get all roles just once.
            var allRoles = await _roleManager.Roles.ToListAsync();
            var allUsers = await _userManager.Users.ToListAsync();

            UsersWithRoles = new List<UserWithRolesViewModel>();
            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var currentRole = userRoles.FirstOrDefault() ?? "No Role";

                UsersWithRoles.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    CurrentRole = currentRole,
                    IsApproved = user.EmailConfirmed,
                    // THE FIX IS HERE: We create a new SelectList for each user,
                    // passing their current role as the "selectedValue".
                    RoleOptions = new SelectList(allRoles, "Name", "Name", currentRole),
                    NewRole = currentRole
                });
            }
        }

        // Handler for updating a user's role
        public async Task<IActionResult> OnPostUpdateRoleAsync(string userId, string newRole)
        {
            if (string.IsNullOrEmpty(newRole))
            {
                TempData["ErrorMessage"] = "Please select a role to update.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);
            await _userManager.UpdateSecurityStampAsync(user);

            TempData["SuccessMessage"] = $"Role for {user.Email} updated successfully.";

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            return RedirectToPage();
        }

        // Handler for approving a user
        public async Task<IActionResult> OnPostApproveUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            TempData["SuccessMessage"] = $"User {user.Email} has been approved.";
            return RedirectToPage();
        }

        // Handler for changing a user's password
        public async Task<IActionResult> OnPostChangePasswordAsync(string userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                TempData["ErrorMessage"] = "New password cannot be empty.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                TempData["SuccessMessage"] = $"Password for '{user.Email}' has been changed successfully.";
            }
            else
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["ErrorMessage"] = $"Failed to change password: {errors}";
            }

            return RedirectToPage();
        }

        // Handler specifically for deleting a user
        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            await _userManager.DeleteAsync(user);
            TempData["SuccessMessage"] = $"User {user.Email} has been deleted.";
            return RedirectToPage();
        }
    }
}