namespace MiniAccountManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalAccounts { get; set; }
        public int ActiveUsers { get; set; }
        public decimal TodaysTransactions { get; set; }
        public int PendingApprovals { get; set; }
    }
}