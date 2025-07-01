using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Data;
using MiniAccountManagement.Models;
using ClosedXML.Excel; // Required for Excel Export
using System;
using System.Collections.Generic;
using System.IO;

namespace MiniAccountManagement.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant,Viewer")]
    public class IndexModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        public List<VoucherListViewModel> VoucherList { get; set; }

        // These properties will bind to the date inputs in our UI.
        // SupportsGet = true allows filtering via URL (e.g., /Vouchers/Index?StartDate=2023-01-01)
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public IndexModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void OnGet()
        {
            // Pass the date range properties to the data access layer to get the filtered list.
            VoucherList = _dataAccess.GetVoucherList(StartDate, EndDate);
        }

        // This handler is called when the "Export to Excel" button is clicked.
        public IActionResult OnPostExportToExcel()
        {
            // Get the filtered list again, based on the same StartDate and EndDate from the page.
            var dataToExport = _dataAccess.GetVoucherList(StartDate, EndDate);

            // Using ClosedXML to create the Excel file in memory
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

                    // Return the Excel file to the browser for download
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }
    }
}