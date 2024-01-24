using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using RealEstate.DataAccess.Data;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;

namespace RealEstate.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public IVillaRepository Villas { get; init; }
        public IVillaNumberRepository VillaNumbers { get; init; }

        public ILocalUserRepository LocalUsers { get; init; }

        public UnitOfWork(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;

            Villas = new VillaRepository(_db);
            VillaNumbers = new VillaNumberRepository(_db);
            LocalUsers = new LocalUserRepository(_db, config);

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