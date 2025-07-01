using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces; 
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;

namespace MiniAccountManagement.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant,Viewer")]
    public class IndexModel : PageModel
    {
        private readonly IVoucherRepository _voucherRepo;

        public List<VoucherListViewModel> VoucherList { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public IndexModel(IVoucherRepository voucherRepo)
        {
            _voucherRepo = voucherRepo;
        }

        public void OnGet()
        {
            VoucherList = _voucherRepo.GetVoucherList(StartDate, EndDate);
        }

        public IActionResult OnPostExportToExcel()
        {
            var dataToExport = _voucherRepo.GetVoucherList(StartDate, EndDate);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Vouchers");
                worksheet.Cell(1, 1).InsertTable(dataToExport, "VoucherList", true);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string excelName = $"VoucherList-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }
    }
}