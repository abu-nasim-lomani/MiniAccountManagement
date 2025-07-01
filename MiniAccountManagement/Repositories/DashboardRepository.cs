using Dapper;
using Microsoft.Extensions.Configuration;
using MiniAccountManagement.Models;
using MiniAccountManagement.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniAccountManagement.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;
        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public DashboardViewModel GetDashboardStats()
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                return con.QueryFirstOrDefault<DashboardViewModel>("sp_GetDashboardStats", commandType: CommandType.StoredProcedure);
            }
        }
    }
}