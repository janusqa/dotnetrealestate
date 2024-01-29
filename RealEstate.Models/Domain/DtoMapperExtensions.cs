using RealEstate.Dto;

namespace RealEstate.Models.Domain
{
    public static class DtoMapperExtensions
    {
        // NB this is an "extension method" for villa model
        // the "this" keyword allows this to appear as a member method
        // of the villa model. It allows us to call it like myVilla.ToDto
        // which looks much better than DomainExtension.ToDto(myVilla).
        // aka it is syntactic sugar over the static method.
        public static VillaDto ToDto(this Villa villa)
        {
            return new VillaDto(
                Id: villa.Id,
                Name: villa.Name,
                Details: villa.Details,
                Rate: villa.Rate,
                Sqft: villa.Sqft,
                Occupancy: villa.Occupancy,
                ImageUrl: villa.ImageUrl,
                Amenity: villa.Amenity
            );
        }

        public static UpdateVillaDto ToUpdateDto(this Villa villa)
        {
            return new UpdateVillaDto(
                Id: villa.Id,
                Name: villa.Name,
                Details: villa.Details,
                Rate: villa.Rate,
                Sqft: villa.Sqft,
                Occupancy: villa.Occupancy,
                ImageUrl: villa.ImageUrl,
                Image: null,
                Amenity: villa.Amenity
            );
        }

        public static VillaNumberDto ToDto(this VillaNumber villaNumber)
        {
            return new VillaNumberDto(
                VillaNo: villaNumber.VillaNo,
                VillaId: villaNumber.VillaId,
                SpecialDetails: villaNumber.SpecialDetails,
                VillaDto: villaNumber.Villa?.ToDto()
            );
        }

        public static AccessTokenDto ToDto(this TokenDto tokenDto)
        {
            return new AccessTokenDto(
                AccessToken: tokenDto.AccessToken
            );
        }
    }
}