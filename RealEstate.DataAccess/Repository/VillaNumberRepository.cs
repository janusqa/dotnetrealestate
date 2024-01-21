using RealEstate.DataAccess.Data;
using RealEstate.Models.Domain;

namespace RealEstate.DataAccess.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}