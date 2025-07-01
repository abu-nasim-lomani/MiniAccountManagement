using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces; // Updated namespace

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IChartOfAccountRepository _coaRepo; // Using specific repository

        [BindProperty]
        public ChartOfAccountModel Account { get; set; } = new(); // Initialized to prevent null warnings

        public DeleteModel(IChartOfAccountRepository coaRepository) // Injecting specific repository
        {
            _coaRepo = coaRepository;
        }

        public IActionResult OnGet(int id)
        {
            var account = _coaRepo.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }

            Account = account;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            // Optional: You can add a check here to see if the account has child accounts or transactions before deleting.
            // For now, we will proceed with the delete action as requested.

            var accountToDelete = _coaRepo.GetAccountById(id);
            if (accountToDelete == null)
            {
                return NotFound();
            }

            _coaRepo.DeleteAccount(id);
            TempData["SuccessMessage"] = $"Account '{accountToDelete.AccountName}' has been deleted successfully.";
            return RedirectToPage("./Index");
        }
    }
}