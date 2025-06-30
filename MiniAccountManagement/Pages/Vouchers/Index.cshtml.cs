using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;
using System.Collections.Generic;

namespace MiniAccountManagement.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant,Viewer")]
    public class IndexModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        public List<VoucherListViewModel> VoucherList { get; set; }
        public IndexModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public void OnGet()
        {
            VoucherList = _dataAccess.GetVoucherList();
        }
    }
}