using MiniAccountManagement.Models;
using System;
using System.Collections.Generic;

namespace MiniAccountManagement.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        void SaveVoucher(VoucherViewModel voucher, string createdByUserId);
        List<VoucherListViewModel> GetVoucherList(DateTime? startDate, DateTime? endDate);
        bool UserHasVouchers(string userId);
    }
}