using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class AuthService : BaseService<ApiResponse>, IAuthService
    {
        private readonly string _url;

        public AuthService(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpAccessor,
            ITokenProvider tokenProvider,
            IApiMessageRequestBuilder messageBuilder,
            string url
        )
            : base(httpClient, httpAccessor, tokenProvider, messageBuilder, url)
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
                }, withBearer: false);
        }

        public async Task<ApiResponse?> LogoutAsync()
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = $"{_url}/logout"
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
                }, withBearer: false);
        }
    }
}