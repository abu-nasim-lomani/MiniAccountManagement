using MiniAccountManagement.Models;

namespace MiniAccountManagement.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        DashboardViewModel GetDashboardStats();
    }
}