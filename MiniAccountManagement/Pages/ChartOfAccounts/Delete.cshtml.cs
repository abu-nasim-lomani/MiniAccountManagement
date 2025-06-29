using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    public class DeleteModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        [BindProperty]
        public ChartOfAccountModel Account { get; set; }

        public DeleteModel(IDataAccess dataAccess)
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
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var accountToDelete = _dataAccess.GetAccountById(id);
            if (accountToDelete == null)
            {
                return NotFound();
            }

            _dataAccess.DeleteAccount(id);

            return RedirectToPage("./Index");
        }
    }
}