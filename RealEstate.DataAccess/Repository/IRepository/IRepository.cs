using System.Linq.Expressions;
using Microsoft.Data.SqlClient;

namespace RealEstate.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracked);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);

        Task<IEnumerable<T>> FromSqlAsync(string sql, List<SqlParameter> sqlParameters);
        Task ExecuteSqlAsync(string sql, List<SqlParameter> sqlParameters);
        Task<IEnumerable<U>> SqlQueryAsync<U>(string sql, List<SqlParameter> sqlParameters);
    }
}