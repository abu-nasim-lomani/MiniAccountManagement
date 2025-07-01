using MiniAccountManagement.Models;
using System.Collections.Generic;

namespace MiniAccountManagement.Repositories.Interfaces
{
    public interface IChartOfAccountRepository
    {
        List<ChartOfAccountModel> GetAllAccounts();
        ChartOfAccountModel GetAccountById(int accountId);
        void AddAccount(ChartOfAccountModel account);
        void UpdateAccount(ChartOfAccountModel account);
        void DeleteAccount(int accountId);
    }
}