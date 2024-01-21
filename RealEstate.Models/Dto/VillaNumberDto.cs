using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Dto
{
    public record VillaNumberDto(
        [Required] int VillaNo,
        string? SpecialDetails
    );

    public record CreateVillaNumberDto(
        [Required] int VillaNo,
        string? SpecialDetails
    );

    public record UpdateVillaNumberDto(
        [Required] int VillaNo,
        string? SpecialDetails
    );
}