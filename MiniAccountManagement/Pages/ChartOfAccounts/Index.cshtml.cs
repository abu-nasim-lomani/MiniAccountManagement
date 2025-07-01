using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniAccountManagement.Repositories.Interfaces; // Updated using statement
using ClosedXML.Excel;
using System.IO;
using System.Linq;

namespace MiniAccountManagement.Pages.ChartOfAccounts
{
    [Authorize]
    public class IndexModel : PageModel
    {
        // --- THE CHANGE IS HERE: Using the specific repository interface ---
        private readonly IChartOfAccountRepository _coaRepo;

        // The constructor now injects the specific repository
        public IndexModel(IChartOfAccountRepository coaRepository)
        {
            _coaRepo = coaRepository;
        }

        public void OnGet() { }

        public JsonResult OnGetAccountsAsJson()
        {
            // Using the new repository to get data
            var allAccounts = _coaRepo.GetAllAccounts();
            var jstreeData = allAccounts.Select(account => new
            {
                id = account.AccountID.ToString(),
                parent = account.ParentAccountID.HasValue ? account.ParentAccountID.ToString() : "#",
                text = $"{account.AccountName} ({account.AccountCode})"
            }).ToList();
            return new JsonResult(jstreeData);
        }

        public IActionResult OnPostExportToExcel()
        {
            // Using the new repository to get data
            var accounts = _coaRepo.GetAllAccounts();

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
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }
    }
}