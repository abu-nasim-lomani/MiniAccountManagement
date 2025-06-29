using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Data;
using System.Linq;
namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    public class IndexModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        public IndexModel(IDataAccess dataAccess) { _dataAccess = dataAccess; }
        public void OnGet() { }
        public JsonResult OnGetAccountsAsJson()
        {
            var allAccounts = _dataAccess.GetAllAccounts();
            var jstreeData = allAccounts.Select(account => new
            {
                id = account.AccountID.ToString(),
                parent = account.ParentAccountID.HasValue ? account.ParentAccountID.ToString() : "#",
                text = $"{account.AccountName} ({account.AccountCode})"
            }).ToList();
            return new JsonResult(jstreeData);
        }
    }
}