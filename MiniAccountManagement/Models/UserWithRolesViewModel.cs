using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MiniAccountManagement.Models
{
    public class UserWithRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string CurrentRole { get; set; }
        public bool IsApproved { get; set; }
        public string NewRole { get; set; }
        public SelectList RoleOptions { get; set; }
    }
}