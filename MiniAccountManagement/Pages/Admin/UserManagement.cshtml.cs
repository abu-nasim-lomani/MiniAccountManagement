using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces; 
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
        // We inject the specific repository needed for our logic.
        private readonly IVoucherRepository _voucherRepo;

        public List<UserWithRolesViewModel> UsersWithRoles { get; set; }

        public UserManagementModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<UserManagementModel> logger,
            IVoucherRepository voucherRepo) // <-- Injecting the correct repository
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _voucherRepo = voucherRepo; // <-- Assigning the repository
        }

        public async Task OnGetAsync()
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
            var result = await _userManager.AddToRoleAsync(user, newRole);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                TempData["SuccessMessage"] = $"Role for {user.Email} updated successfully.";
                _logger.LogInformation("Role for user {UserEmail} updated to {NewRole} by admin {AdminEmail}.", user.Email, newRole, User.Identity.Name);
            }
            else
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["ErrorMessage"] = $"Failed to update role: {errors}";
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
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.Email} has been approved.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve user.";
            }

            return RedirectToPage();
        }

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

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (_voucherRepo.UserHasVouchers(userId))
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
            var allUsers = await _userManager.Users.ToListAsync();
            var exportList = new List<object>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                exportList.Add(new { Email = user.Email, Role = roles.FirstOrDefault() ?? "No Role", IsApproved = user.EmailConfirmed });
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                worksheet.Cell(1, 1).InsertTable(exportList, "UserList", true);
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