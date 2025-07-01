using Dapper;
using Microsoft.Extensions.Configuration;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MiniAccountManagement.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly string _connectionString;
        public VoucherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void SaveVoucher(VoucherViewModel voucher, string createdByUserId)
        {
            var detailsTable = new DataTable();
            detailsTable.Columns.Add("AccountID", typeof(int));
            detailsTable.Columns.Add("DebitAmount", typeof(decimal));
            detailsTable.Columns.Add("CreditAmount", typeof(decimal));

            foreach (var detail in voucher.Details)
            {
                detailsTable.Rows.Add(detail.AccountID, detail.DebitAmount ?? 0, detail.CreditAmount ?? 0);
            }

            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    voucher.VoucherDate,
                    voucher.VoucherType,
                    voucher.ReferenceNo,
                    voucher.Narration,
                    CreatedBy = createdByUserId,
                    VoucherDetails = detailsTable.AsTableValuedParameter("dbo.VoucherDetailType")
                };
                con.Execute("sp_SaveVoucher", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public List<VoucherListViewModel> GetVoucherList(DateTime? startDate, DateTime? endDate)
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                return con.Query<VoucherListViewModel>("sp_GetVoucherList", new { StartDate = startDate, EndDate = endDate }, commandType: CommandType.StoredProcedure).AsList();
            }
        }

        public bool UserHasVouchers(string userId)
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                return con.QuerySingle<bool>("sp_CheckUserHasVouchers", new { UserId = userId }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}