using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using RealEstate.DataAccess.Data;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Models.Identity;

namespace RealEstate.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IVillaRepository Villas { get; init; }
        public IVillaNumberRepository VillaNumbers { get; init; }
        public ILocalUserRepository LocalUsers { get; init; }
        public IApplicationUserRepository ApplicationUsers { get; init; }

        public UnitOfWork(
            ApplicationDbContext db,
            IConfiguration config,
            UserManager<ApplicationUser> um,
            IUserStore<ApplicationUser> us
        )
        {
            _db = db;

            Villas = new VillaRepository(_db);
            VillaNumbers = new VillaNumberRepository(_db);
            LocalUsers = new LocalUserRepository(_db, config);
            ApplicationUsers = new ApplicationUserRepository(_db, config, um, us);
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