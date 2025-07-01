using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces; // Using the new Repositories namespace
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniAccountManagement.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class VoucherEntryModel : PageModel
    {
        // --- THE CHANGE IS HERE: Injecting two specific repositories ---
        private readonly IVoucherRepository _voucherRepo;
        private readonly IChartOfAccountRepository _coaRepo;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public VoucherViewModel Voucher { get; set; } = new();
        public SelectList AccountOptions { get; set; } = null!;

        // The constructor now injects the repositories this page needs.
        public VoucherEntryModel(
            IVoucherRepository voucherRepo,
            IChartOfAccountRepository coaRepo,
            UserManager<IdentityUser> userManager)
        {
            _voucherRepo = voucherRepo;
            _coaRepo = coaRepo;
            _userManager = userManager;
        }

        private void PopulateAccountOptions()
        {
            // Use the Chart of Account repository to get the account list
            var accounts = _coaRepo.GetAllAccounts();

            // Format the list to show "Code - Name" for a better UX
            var accountListForDropdown = accounts.Select(a => new {
                a.AccountID,
                DisplayText = $"{a.AccountCode} - {a.AccountName}"
            }).ToList();

            AccountOptions = new SelectList(accountListForDropdown, "AccountID", "DisplayText");
        }

        public void OnGet()
        {
            PopulateAccountOptions();
            // Initialize with two blank rows for the user
            Voucher.Details.Add(new VoucherDetailViewModel());
            Voucher.Details.Add(new VoucherDetailViewModel());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove empty lines before validation
            Voucher.Details.RemoveAll(d => d.AccountID == null);

            // Business logic validation
            if (Voucher.Details.Count < 2)
            {
                ModelState.AddModelError("", "A voucher must have at least two entries.");
            }

            decimal totalDebit = Voucher.Details.Sum(d => d.DebitAmount ?? 0);
            decimal totalCredit = Voucher.Details.Sum(d => d.CreditAmount ?? 0);

            if (totalDebit != totalCredit)
            {
                ModelState.AddModelError("", "Total Debit must be equal to Total Credit.");
            }
            if (totalDebit == 0)
            {
                ModelState.AddModelError("", "Total transaction amount cannot be zero.");
            }

            if (!ModelState.IsValid)
            {
                // If validation fails, repopulate dropdown and show page with errors
                PopulateAccountOptions();
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Handle case where user is not found, though [Authorize] should prevent this.
                return Forbid();
            }

            // Use the Voucher repository to save the voucher
            _voucherRepo.SaveVoucher(Voucher, userId);

            TempData["SuccessMessage"] = "Voucher saved successfully!";
            return RedirectToPage();
        }
    }
}