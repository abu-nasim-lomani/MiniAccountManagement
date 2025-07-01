using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces;
using System.Linq;

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class EditModel : PageModel
    {
        private readonly IChartOfAccountRepository _coaRepo;

        [BindProperty]
        public ChartOfAccountModel Account { get; set; } = null!;
        public SelectList ParentAccountOptions { get; set; } = null!;

        public EditModel(IChartOfAccountRepository coaRepository)
        {
            _coaRepo = coaRepository;
        }

        // A private helper to populate the dropdown, preventing code duplication
        private void PopulateParentAccountDropdown(int currentAccountId)
        {
            // Exclude the current account from the list of possible parents
            var accounts = _coaRepo.GetAllAccounts().Where(a => a.AccountID != currentAccountId).ToList();

            // Format the display text to show "Code - Name"
            var accountListForDropdown = accounts.Select(a => new {
                a.AccountID,
                DisplayText = $"{a.AccountCode} - {a.AccountName}"
            }).ToList();

            ParentAccountOptions = new SelectList(accountListForDropdown, "AccountID", "DisplayText", Account.ParentAccountID);
        }

        public IActionResult OnGet(int id)
        {
            var account = _coaRepo.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }

            Account = account;
            PopulateParentAccountDropdown(id);
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // If validation fails, repopulate the dropdown before showing the page again
                PopulateParentAccountDropdown(Account.AccountID);
                return Page();
            }

            _coaRepo.UpdateAccount(Account);
            TempData["SuccessMessage"] = "Account updated successfully!";
            return RedirectToPage("./Index");
        }
    }
}