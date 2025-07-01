using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces;

namespace MiniAccountManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardViewModel DashboardStats { get; set; }

        public IndexModel(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                DashboardStats = _dashboardRepo.GetDashboardStats();
                return Page();
            }
            else
            {
                // If LOGGED OUT: Redirect them to our public landing page.
                return RedirectToPage("/Landing");
            }
        }
    }
}