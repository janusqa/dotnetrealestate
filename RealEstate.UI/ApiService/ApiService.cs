using RealEstate.UI.Services;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.ApiService
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public IVillaService Villas { get; init; }
        public IVillaNumberService VillaNumbers { get; init; }
        public IAuthService LocalUsers { get; init; }


        public ApiService(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            var urlBase = configuration.GetValue<string>("ServiceUrls:VillaApi");

            Villas = new VillaService(_httpClient, _httpContextAccessor, $@"{urlBase}/api/{SD.ApiVersion}/villas");
            VillaNumbers = new VillaNumberService(_httpClient, _httpContextAccessor, $@"{urlBase}/api/{SD.ApiVersion}/villanumbers");
            LocalUsers = new AuthService(_httpClient, _httpContextAccessor, $@"{urlBase}/api/{SD.ApiVersion}/users");

        }
    }
}