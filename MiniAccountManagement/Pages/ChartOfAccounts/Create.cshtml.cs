using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; 
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
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

        public void OnGet()
        {
            var accounts = _dataAccess.GetAllAccounts();
            ParentAccountOptions = new SelectList(accounts, "AccountID", "AccountName");
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                OnGet(); 
                return Page();
            }

            _dataAccess.AddAccount(Account);

            return RedirectToPage("./Index");
        }
    }
}