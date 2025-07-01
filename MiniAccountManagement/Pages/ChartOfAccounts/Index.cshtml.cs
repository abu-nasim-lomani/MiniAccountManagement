using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Data;
using ClosedXML.Excel; // Required for Excel Export
namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    [Authorize]
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

        // --- NEW HANDLER FOR EXPORTING TO EXCEL ---
        public IActionResult OnPostExportToExcel()
        {
            var accounts = _dataAccess.GetAllAccounts();

            var dataForExport = accounts.Select(a => new {
                a.AccountCode,
                a.AccountName,
                ParentAccountCode = a.ParentAccountID.HasValue
                    ? accounts.FirstOrDefault(p => p.AccountID == a.ParentAccountID)?.AccountCode
                    : "N/A"
            }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Chart of Accounts");
                worksheet.Cell(1, 1).InsertTable(dataForExport, "ChartOfAccounts", true);

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string excelName = $"ChartOfAccounts-{System.DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    // Return the Excel file to the browser for download.
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }
    }
}