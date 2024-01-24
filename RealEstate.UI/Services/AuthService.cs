using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;

namespace RealEstate.UI.Services
{
    public class AuthService : BaseService<ApiResponse>, IAuthService
    {
        public AuthService(IHttpClientFactory httpClient, string url) : base(httpClient, url)
        {

        }

    }
}