using RealEstate.DataAccess.Repository;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RealEstate.DataAccess.UnitOfWork.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IVillaRepository Villas { get; init; }

        int Complete();

        DatabaseFacade Context();
    }
}