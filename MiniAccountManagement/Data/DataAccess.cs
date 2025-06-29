using Dapper;
using MiniAccountManagement.Models;
using System.Data;
using System.Data.SqlClient;
namespace MiniAccountManagement.Data
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        // ... All 5 methods (GetAllAccounts, GetAccountById, etc.) using Dapper go here ...
        // The code for the methods is exactly the same as the previous step.
        public List<ChartOfAccountModel> GetAllAccounts()
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                return con.Query<ChartOfAccountModel>("sp_ManageChartOfAccounts", new { Action = "SELECT_ALL" }, commandType: CommandType.StoredProcedure).ToList();
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
                var parameters = new { Action = "CREATE", account.AccountName, account.AccountCode, account.ParentAccountID };
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