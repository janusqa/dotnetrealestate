using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class AuthService : BaseService<ApiResponse>, IAuthService
    {
        private readonly string _url;
        public AuthService(IHttpClientFactory httpClient, ITokenProvider tokenProvider, string url)
            : base(httpClient, tokenProvider, url)
        {
            _url = url;
        }

        public async Task<ApiResponse?> LoginAsync<U>(U dto)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.POST,
                    Data = dto,
                    Url = $"{_url}/login"
                });
        }

        public async Task<ApiResponse?> RegisterAsync<U>(U dto)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.POST,
                    Data = dto,
                    Url = $"{_url}/register"
                });
        }
    }
}