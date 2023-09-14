using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BookManagementSystem.Infrastructure
{
    public class DbHelper : IDisposable
    {
        private readonly IConfiguration _configuration;
        public DbHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SqlConnection GetConn()
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            return conn;
        }

        

        public async Task<IEnumerable<T>> ExecuteQuery<T>(string sql,DynamicParameters param,bool isproc=false)
        {
            SqlConnection conn = GetConn();
            return await conn.QueryAsync<T>(sql, param, commandType: (isproc) ? CommandType.StoredProcedure : CommandType.Text);
           // return null;
        }
        public void Execute(string sql, DynamicParameters param, bool isproc = false)
        {
            var conn=  GetConn();
            conn.Execute(sql, param, commandType: (isproc) ? CommandType.StoredProcedure : CommandType.Text);
        }
        public void Dispose()
        {
            if (GetConn().State == ConnectionState.Open)
                GetConn().Close();
        }
    }
}
