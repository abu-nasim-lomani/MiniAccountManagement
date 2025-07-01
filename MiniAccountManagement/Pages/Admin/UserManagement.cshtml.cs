// Pages/Admin/UserManagement.cshtml.cs

using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;
using System.Security.Claims;

namespace MiniAccountManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserManagementModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<UserManagementModel> _logger;
        private readonly IDataAccess _dataAccess; // This field was declared but not initialized.

        public List<UserWithRolesViewModel> UsersWithRoles { get; set; }

        // --- THE FIX IS HERE ---
        // We add IDataAccess to the constructor parameters.
        public UserManagementModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<UserManagementModel> logger,
            IDataAccess dataAccess) // <-- Add this parameter
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _dataAccess = dataAccess; // <-- And assign it here.
        }

        private async Task PopulateUsersList()
        {
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
                    RoleOptions = new SelectList(allRoles, "Name", "Name", currentRole),
                    NewRole = currentRole
                });
            }
        }

        public async Task OnGetAsync()
        {
            await PopulateUsersList();
        }

        public async Task<IActionResult> OnPostUpdateRoleAsync(string userId, string newRole)
        {
            // ... The rest of the methods remain the same as they are correct ...
            if (string.IsNullOrEmpty(newRole))
            {
                TempData["ErrorMessage"] = "Please select a role to update.";
                return RedirectToPage();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, newRole);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                TempData["SuccessMessage"] = $"Role for {user.Email} updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update role.";
            }

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            TempData["SuccessMessage"] = $"User {user.Email} has been approved.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync(string userId, string newPassword)
        {
            // ... code is correct
            if (string.IsNullOrEmpty(newPassword)) { TempData["ErrorMessage"] = "New password cannot be empty."; return RedirectToPage(); }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded) { await _userManager.UpdateSecurityStampAsync(user); TempData["SuccessMessage"] = $"Password for '{user.Email}' has been changed successfully."; }
            else { string errors = string.Join(", ", result.Errors.Select(e => e.Description)); TempData["ErrorMessage"] = $"Failed to change password: {errors}"; }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // This check now correctly uses the injected _dataAccess field
            if (_dataAccess.UserHasVouchers(userId))
            {
                TempData["ErrorMessage"] = $"Cannot delete user '{user.Email}'. This user has existing vouchers linked to them.";
                return RedirectToPage();
            }

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.Email} has been deleted.";
            }
            else
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["ErrorMessage"] = $"Failed to delete user: {errors}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportToExcelAsync()
        {
            // ... code is correct
            await PopulateUsersList();
            var dataForExport = UsersWithRoles.Select(u => new { u.Email, u.CurrentRole, u.IsApproved }).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                worksheet.Cell(1, 1).InsertTable(dataForExport, "UserList", true);
                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"UserList-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
    }
}