// DTOs. Used to access our models. 
// We do not access models directly in our applications we
// access DTOS. This is so we can scope/filter the info
// accessed. Eg. if our model for DB purposes consist of 
// a CreatedDate field we may not want to send this back
// So in our controller we gaurd the info the model returns
// Via a DTO.
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Dto
{
    public record LocalUserDto(
        int Id,
        [Required][MaxLength(30)] string UserName,
        [Required][MaxLength(30)] string Role,
        [MaxLength(30)] string? Name
    );

    public record CreateLocalUserDto(
        [Required][MaxLength(30)] string UserName,
        [Required][MaxLength(30)] string Password,
        [Required][MaxLength(30)] string Role,
        [MaxLength(30)] string? Name
    );

    public record UpdateLocalUserDto(
        int Id,
        [Required][MaxLength(30)] string UserName,
        [Required][MaxLength(30)] string Password,
        [Required][MaxLength(30)] string Role,
        [MaxLength(30)] string? Name
    );

    public record LocalUserLoginRequestDto(
        [Required][MaxLength(30)] string UserName,
        [Required][MaxLength(30)] string Password
    );

    public record LocalUserLoginResponseDto(
        [Required] LocalUserDto User,
        [Required] string Token
    );

}