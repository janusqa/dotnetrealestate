using System.Linq.Expressions;
using RealEstate.DataAccess.Data;
using RealEstate.DataAccess.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace RealEstate.DataAccess.Repository
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
        }

        public T? Get(int Id)
        {
            return _db.Set<T>().Find(Id);
        }

        public IEnumerable<T> GetAll()
        {
            return _db.Set<T>().ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _db.Set<T>().Where(predicate);
        }

        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _db.Set<T>().AddRange(entities);
        }

        public void Remove(T entity)
        {
            _db.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
        }

        public async Task<IEnumerable<T>> FromSqlAsync(string sql, List<SqlParameter> sqlParameters)
        {
            return await _db.Set<T>().FromSqlRaw(sql, sqlParameters.ToArray()).ToListAsync();
        }

        public async Task ExecuteSqlAsync(string sql, List<SqlParameter> sqlParameters)
        {
            await _db.Database.ExecuteSqlRawAsync(sql, sqlParameters.ToArray());
        }

        public async Task<IEnumerable<U>> SqlQueryAsync<U>(string sql, List<SqlParameter> sqlParameters)
        {
            return await _db.Database.SqlQueryRaw<U>(sql, sqlParameters.ToArray()).ToListAsync();
        }
    }
}