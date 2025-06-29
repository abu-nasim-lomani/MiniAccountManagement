using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;
namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    public class IndexModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        public List<ChartOfAccountModel> Accounts { get; set; }
        public IndexModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public void OnGet()
        {
            Accounts = _dataAccess.GetAllAccounts();
        }
    }
}
