namespace MiniAccountManagement.Models
{
    public class ChartOfAccountModel
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public int? ParentAccountID { get; set; } 
    }
}