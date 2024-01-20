using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RealEstate.DataAccess.Data;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;

namespace RealEstate.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IVillaRepository Villas { get; init; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Villas = new VillaRepository(_db);
        }

        public async Task<int> Complete()
        {
            return await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public IDbContextTransaction Transaction() => _db.Database.BeginTransaction();

    }
}