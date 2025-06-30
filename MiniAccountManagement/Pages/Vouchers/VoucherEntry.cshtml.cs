using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniAccountManagement.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class VoucherEntryModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public VoucherViewModel Voucher { get; set; }

        public SelectList AccountOptions { get; set; }

        public VoucherEntryModel(IDataAccess dataAccess, UserManager<IdentityUser> userManager)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
        }

        public void OnGet()
        {
            PopulateAccountOptions();

            Voucher = new VoucherViewModel();
            Voucher.Details.Add(new VoucherDetailViewModel());
            Voucher.Details.Add(new VoucherDetailViewModel());
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Voucher.Details.RemoveAll(d => d.AccountID == null);

            if (Voucher.Details.Count < 2)
            {
                ModelState.AddModelError("", "A voucher must have at least two entries (one debit and one credit).");
            }

            decimal totalDebit = Voucher.Details.Sum(d => d.DebitAmount ?? 0);
            decimal totalCredit = Voucher.Details.Sum(d => d.CreditAmount ?? 0);

            if (totalDebit != totalCredit)
            {
                ModelState.AddModelError("", "Total Debit must be equal to Total Credit.");
            }
            if (totalDebit == 0)
            {
                ModelState.AddModelError("", "Total amount cannot be zero.");
            }

            if (!ModelState.IsValid)
            {
                PopulateAccountOptions();
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _dataAccess.SaveVoucher(Voucher, userId);

            TempData["SuccessMessage"] = "Voucher saved successfully!";

            return RedirectToPage();
        }

        private void PopulateAccountOptions()
        {
            var accounts = _dataAccess.GetAllAccounts();
            AccountOptions = new SelectList(accounts, "AccountID", "AccountName");
        }
    }
}