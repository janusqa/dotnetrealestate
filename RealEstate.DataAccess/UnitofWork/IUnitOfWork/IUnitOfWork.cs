using RealEstate.DataAccess.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace RealEstate.DataAccess.UnitOfWork.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IVillaRepository Villas { get; init; }
        IVillaNumberRepository VillaNumbers { get; init; }

        Task<int> Complete();

        IDbContextTransaction Transaction();
    }
}