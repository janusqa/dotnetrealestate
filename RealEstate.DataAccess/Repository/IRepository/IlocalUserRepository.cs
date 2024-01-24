using RealEstate.Models.Domain;
using RealEstate.DataAccess.Repository.IRepository;
using RealEstate.Dto;

namespace RealEstate.DataAccess.Repository
{
    public interface ILocalUserRepository : IRepository<LocalUser>
    {
        Task<bool> IsUinqueUser(string UserName);
        Task<LocalUserLoginResponseDto?> Login(LocalUserLoginRequestDto loginRequestDto);
        Task<LocalUserLoginResponseDto?> Register(CreateLocalUserDto userDto);
    }
}