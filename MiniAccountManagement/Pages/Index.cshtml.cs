using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Data; 
using MiniAccountManagement.Models;

namespace MiniAccountManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        public DashboardViewModel DashboardStats { get; set; }

        public IndexModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void OnGet()
        {
            DashboardStats = _dataAccess.GetDashboardStats();
        }
    }
}