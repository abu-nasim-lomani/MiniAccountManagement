using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces;

namespace MiniAccountManagement.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IDashboardRepository _dashboardRepo;
        public DashboardViewModel DashboardStats { get; set; } = new();

        public DashboardModel(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        // This OnGet method acts as a "traffic controller".
        public IActionResult OnGet()
        {
            // Check if the user is authenticated.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // If LOGGED IN: Fetch dashboard data and show the current page.
                DashboardStats = _dashboardRepo.GetDashboardStats();
                return Page();
            }
            else
            {
                // If LOGGED OUT: Redirect them to the routable "/Landing" page.
                // DO NOT redirect to a layout file like "_PublicLayout".
                return RedirectToPage("/Home");
            }
        }
    }
}