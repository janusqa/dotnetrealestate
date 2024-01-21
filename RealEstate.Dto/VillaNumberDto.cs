using System.ComponentModel.DataAnnotations;

namespace RealEstate.Dto
{
    public record VillaNumberDto(
        [Required] int VillaNo,
        [Required] int VillaId,
        string? SpecialDetails
    );

    public record CreateVillaNumberDto(
        [Required] int VillaNo,
        [Required] int VillaId,
        string? SpecialDetails
    );

    public record UpdateVillaNumberDto(
        [Required] int VillaNo,
        [Required] int VillaId,
        string? SpecialDetails
    );
}