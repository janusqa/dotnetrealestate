using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Dto
{
    public record VillaNumberDto(
        [Required] int VillaNo,
        [Required] int VillaId,
        string? SpecialDetails,
        VillaDto? VillaDto
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