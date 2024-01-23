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
                villaDto.Name,
                villaDto.Details,
                villaDto.Rate,
                villaDto.Sqft,
                villaDto.Occupancy,
                villaDto.ImageUrl,
                villaDto.Amenity
            );
        }

        // This is only useful if we support PATCH method
        public static UpdateVillaDto ToUpdateDto(this VillaDto villaDto)
        {
            return new UpdateVillaDto(
                villaDto.Id,
                villaDto.Name,
                villaDto.Details,
                villaDto.Rate,
                villaDto.Sqft,
                villaDto.Occupancy,
                villaDto.ImageUrl ?? "",
                villaDto.Amenity
            );
        }
    }
}