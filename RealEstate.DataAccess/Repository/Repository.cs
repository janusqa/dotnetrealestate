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

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracked = true)
        {
            return tracked
                ? await _db.Set<T>().Where(predicate).FirstOrDefaultAsync()
                    : await _db.Set<T>().Where(predicate).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate)
        {
            return predicate is not null
                ? await _db.Set<T>().Where(predicate).ToListAsync()
                    : await _db.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _db.Set<T>().AddRangeAsync(entities);
        }

        public async Task RemoveAsync(T entity)
        {
            _db.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
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