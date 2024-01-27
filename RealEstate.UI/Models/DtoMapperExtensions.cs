using RealEstate.Dto;

namespace RealEstate.UI.Models
{
    public static class DtoMapperExtensions
    {
        // NB this is an "extension method" for villa model
        // the "this" keyword allows this to appear as a member method
        // of the villa model. It allows us to call it like myVilla.ToDto
        // which looks much better than DomainExtension.ToDto(myVilla).
        // aka it is syntactic sugar over the static method.
        public static CreateVillaDto ToCreateDto(this VillaDto villaDto)
        {
            return new CreateVillaDto(
                Name: villaDto.Name,
                Details: villaDto.Details,
                Rate: villaDto.Rate,
                Sqft: villaDto.Sqft,
                Occupancy: villaDto.Occupancy,
                ImageUrl: villaDto.ImageUrl,
                Image: null,
                Amenity: villaDto.Amenity
            );
        }

        public static UpdateVillaDto ToUpdateDto(this VillaDto villaDto)
        {
            return new UpdateVillaDto(
                Id: villaDto.Id,
                Name: villaDto.Name,
                Details: villaDto.Details,
                Rate: villaDto.Rate,
                Sqft: villaDto.Sqft,
                Occupancy: villaDto.Occupancy,
                ImageUrl: villaDto.ImageUrl,
                Image: null,
                Amenity: villaDto.Amenity
            );
        }

        public static CreateVillaNumberDto ToCreateDto(this VillaNumberDto villaNumberDto)
        {
            return new CreateVillaNumberDto(
                VillaNo: villaNumberDto.VillaNo,
                VillaId: villaNumberDto.VillaId,
                SpecialDetails: villaNumberDto.SpecialDetails
            );
        }

        public static UpdateVillaNumberDto ToUpdateDto(this VillaNumberDto villaNumberDto)
        {
            return new UpdateVillaNumberDto(
                VillaNo: villaNumberDto.VillaNo,
                VillaId: villaNumberDto.VillaId,
                SpecialDetails: villaNumberDto.SpecialDetails
            );
        }
    }
}