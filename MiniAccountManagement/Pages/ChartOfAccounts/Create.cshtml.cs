using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;
using System.Linq;

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        [BindProperty]
        public ChartOfAccountModel Account { get; set; }

        public SelectList ParentAccountOptions { get; set; }

        public CreateModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        private void PopulateParentAccountDropdown()
        {
            var accounts = _dataAccess.GetAllAccounts();

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

            _dataAccess.AddAccount(Account);
            TempData["SuccessMessage"] = "Account created successfully!";
            return RedirectToPage("./Index");
        }
    }
}