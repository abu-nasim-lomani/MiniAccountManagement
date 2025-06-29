using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    public class EditModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        [BindProperty]
        public ChartOfAccountModel Account { get; set; }

        public SelectList ParentAccountOptions { get; set; }

        public EditModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IActionResult OnGet(int id)
        {
            Account = _dataAccess.GetAccountById(id);

            if (Account == null)
            {
                return NotFound();
            }
            var accountsForDropdown = _dataAccess.GetAllAccounts().Where(a => a.AccountID != id).ToList();
            ParentAccountOptions = new SelectList(accountsForDropdown, "AccountID", "AccountName");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                var accountsForDropdown = _dataAccess.GetAllAccounts().Where(a => a.AccountID != Account.AccountID).ToList();
                ParentAccountOptions = new SelectList(accountsForDropdown, "AccountID", "AccountName");
                return Page();
            }
            _dataAccess.UpdateAccount(Account);

            return RedirectToPage("./Index");
        }
    }
}