using System.Linq.Expressions;
using Microsoft.Data.SqlClient;

namespace RealEstate.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T? Get(int Id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        Task<IEnumerable<T>> FromSqlAsync(string sql, List<SqlParameter> sqlParameters);
        Task ExecuteSqlAsync(string sql, List<SqlParameter> sqlParameters);

        Task<IEnumerable<U>> SqlQueryAsync<U>(string sql, List<SqlParameter> sqlParameters);
    }
}