// Pages/Landing.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiniAccountManagement.Pages
{
    public class LandingModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Check if the user is already logged in
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // If they are, redirect them straight to the new dashboard page.
                return RedirectToPage("/Dashboard");
            }

            // Otherwise, show the landing page content.
            return Page();
        }
    }
}