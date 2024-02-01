using RealEstate.UI.Models;

namespace RealEstate.UI.Services.IServices
{
    public interface IAuthService : IBaseService<ApiResponse>
    {
        Task<ApiResponse?> LoginAsync<U>(U dto);
        Task<ApiResponse?> RegisterAsync<U>(U dto);
        Task<ApiResponse?> LogoutAsync();
    }
}