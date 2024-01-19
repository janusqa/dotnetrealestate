using Microsoft.EntityFrameworkCore.Infrastructure;
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

        public int Complete()
        {
            return _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public DatabaseFacade Context() => _db.Database;

    }
}