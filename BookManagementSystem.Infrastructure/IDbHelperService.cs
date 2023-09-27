using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagementSystem.Infrastructure
{
    public interface IDbHelperService
    {
        Task<IEnumerable<T>> ExecuteQuery<T>(string sql, DynamicParameters param, bool isproc = false);
        void Execute(string sql, DynamicParameters param, bool isproc = false);
    }
}
