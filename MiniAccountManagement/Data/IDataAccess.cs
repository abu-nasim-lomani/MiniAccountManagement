using MiniAccountManagement.Models;
using System.Collections.Generic;

namespace MiniAccountManagement.Data
{
    public interface IDataAccess
    {
        List<ChartOfAccountModel> GetAllAccounts();
        List<VoucherListViewModel> GetVoucherList();
        ChartOfAccountModel GetAccountById(int accountId);
        void AddAccount(ChartOfAccountModel account);
        void UpdateAccount(ChartOfAccountModel account);
        void DeleteAccount(int accountId);
        void SaveVoucher(VoucherViewModel voucher, string createdByUserId);
    }
}