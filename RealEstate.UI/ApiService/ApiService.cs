using RealEstate.UI.Services;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.ApiService
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ITokenProvider _tokenProvider;


        public IVillaService Villas { get; init; }
        public IVillaNumberService VillaNumbers { get; init; }
        public IAuthService LocalUsers { get; init; }
        public IAuthService ApplicationUsers { get; init; }


        public ApiService(
            IHttpClientFactory httpClient,
            ITokenProvider tokenProvider,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;

            var urlBase = configuration.GetValue<string>("ServiceUrls:VillaApi");

            Villas = new VillaService(_httpClient, _tokenProvider, $@"{urlBase}/api/{SD.ApiVersion}/villas");
            VillaNumbers = new VillaNumberService(_httpClient, _tokenProvider, $@"{urlBase}/api/{SD.ApiVersion}/villanumbers");
            LocalUsers = new AuthService(_httpClient, _tokenProvider, $@"{urlBase}/api/{SD.ApiVersion}/users");
            ApplicationUsers = new AuthService(_httpClient, _tokenProvider, $@"{urlBase}/api/{SD.ApiVersion}/users");

        }
    }
}