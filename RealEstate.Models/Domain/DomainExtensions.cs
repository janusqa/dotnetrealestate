using RealEstate.Models.Dto;

namespace RealEstate.Models.Domain
{
    public static class DomainExtensions
    {
        // NB this is an "extension method" for villa model
        // the "this" keyword allows this to appear as a member method
        // of the villa model. It allows us to call it like myVilla.ToDto
        // which looks much better than DomainExtension.ToDto(myVilla).
        // aka it is syntactic sugar over the static method.
        public static VillaDto ToDto(this Villa villa)
        {
            return new VillaDto(
                villa.Id,
                villa.Name,
                villa.Details,
                villa.Rate,
                villa.Sqft,
                villa.Occupancy,
                villa.ImageUrl,
                villa.Amenity
            );
        }

        public static UpdateVillaDto ToUpdateDto(this Villa villa)
        {
            return new UpdateVillaDto(
                villa.Id,
                villa.Name,
                villa.Details,
                villa.Rate,
                villa.Sqft,
                villa.Occupancy,
                villa.ImageUrl,
                villa.Amenity
            );
        }

        public static VillaNumberDto ToDto(this VillaNumber villaNumber)
        {
            return new VillaNumberDto(
                villaNumber.VillaNo,
                villaNumber.SpecialDetails
            );
        }
    }
}