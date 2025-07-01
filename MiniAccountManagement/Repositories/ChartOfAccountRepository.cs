using Dapper;
using Microsoft.Extensions.Configuration;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace MiniAccountManagement.Repositories
{
    public class ChartOfAccountRepository : IChartOfAccountRepository
    {
        private readonly string _connectionString;

        public ChartOfAccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<ChartOfAccountModel> GetAllAccounts()
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                return con.Query<ChartOfAccountModel>("sp_ManageChartOfAccounts", new { Action = "SELECT_ALL" }, commandType: CommandType.StoredProcedure).AsList();
            }
        }

        public ChartOfAccountModel GetAccountById(int accountId)
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                return con.QueryFirstOrDefault<ChartOfAccountModel>("sp_ManageChartOfAccounts", new { Action = "SELECT_BY_ID", AccountID = accountId }, commandType: CommandType.StoredProcedure);
            }
        }

        public void AddAccount(ChartOfAccountModel account)
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                var parameters = new { Action = "CREATE", account.AccountName, account.ParentAccountID };
                con.Execute("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void UpdateAccount(ChartOfAccountModel account)
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                var parameters = new { Action = "UPDATE", account.AccountID, account.AccountName, account.AccountCode, account.ParentAccountID };
                con.Execute("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void DeleteAccount(int accountId)
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                con.Execute("sp_ManageChartOfAccounts", new { Action = "DELETE", AccountID = accountId }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}