using RealEstate.DataAccess.Repository.IRepository;
using RealEstate.Dto;
using RealEstate.Models.Identity;

namespace RealEstate.DataAccess.Repository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task<bool> IsUinqueUser(string UserName);
        Task<ApplicationUserLoginResponseDto?> Login(ApplicationUserLoginRequestDto loginRequestDto);
        Task<ApplicationUserLoginResponseDto?> Register(CreateApplicationUserDto userDto);
    }
}