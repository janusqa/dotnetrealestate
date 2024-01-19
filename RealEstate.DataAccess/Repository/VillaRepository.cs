using RealEstate.DataAccess.Data;
using RealEstate.Models.Domain;

namespace RealEstate.DataAccess.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        public VillaRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}