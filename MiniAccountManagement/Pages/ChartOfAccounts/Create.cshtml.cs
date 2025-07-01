using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces; // <-- Updated using statement
using System.Linq;

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateModel : PageModel
    {
        // --- THE CHANGE IS HERE: Using the specific repository interface ---
        private readonly IChartOfAccountRepository _coaRepo;

        [BindProperty]
        public ChartOfAccountModel Account { get; set; }

        public SelectList ParentAccountOptions { get; set; }

        // The constructor now injects the specific repository
        public CreateModel(IChartOfAccountRepository coaRepository)
        {
            _coaRepo = coaRepository;
        }

        private void PopulateParentAccountDropdown()
        {
            // Using the new repository to get data
            var accounts = _coaRepo.GetAllAccounts();

            var accountListForDropdown = accounts.Select(a => new {
                a.AccountID,
                DisplayText = $"{a.AccountCode} - {a.AccountName}"
            }).ToList();

            ParentAccountOptions = new SelectList(accountListForDropdown, "AccountID", "DisplayText");
        }

        public void OnGet()
        {
            PopulateParentAccountDropdown();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                PopulateParentAccountDropdown();
                return Page();
            }

            // Using the new repository to save data
            _coaRepo.AddAccount(Account);

            TempData["SuccessMessage"] = "Account created successfully!";
            return RedirectToPage("./Index");
        }
    }
}