// DTOs. Used to access our models. 
// We do not access models directly in our applications we
// access DTOS. This is so we can scope/filter the info
// accessed. Eg. if our model for DB purposes consist of 
// a CreatedDate field we may not want to send this back
// So in our controller we gaurd the info the model returns
// Via a DTO.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstate.Dto
{
    public record VillaDto(
        int Id,
        [Required][MaxLength(30)] string Name,
        string? Details,
        [Required] double Rate,
        int Sqft,
        int Occupancy,
        string? ImageUrl,
        string? Amenity
    );

    public record CreateVillaDto(
        [Required][MaxLength(30)] string Name,
        string? Details,
        [Required] double Rate,
        int Sqft,
        int Occupancy,
        string? ImageUrl,
        IFormFile? Image,
        string? Amenity
    );

    public record UpdateVillaDto(
        [Required] int Id,
        [Required][MaxLength(30)] string Name,
        string? Details,
        [Required] double Rate,
        [Required] int Sqft,
        [Required] int Occupancy,
        string? ImageUrl,
        string? ImageLocalPath,
        IFormFile? Image,
        string? Amenity
    );
}