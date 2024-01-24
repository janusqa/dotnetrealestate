using RealEstate.UI.Services;
using RealEstate.UI.Services.IServices;

namespace RealEstate.UI.ApiService
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClient;

        public IVillaService Villas { get; init; }
        public IVillaNumberService VillaNumbers { get; init; }
        public IAuthService LocalUsers { get; init; }


        public ApiService(IHttpClientFactory httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var urlBase = configuration.GetValue<string>("ServiceUrls:VillaApi");

            Villas = new VillaService(_httpClient, $@"{urlBase}/api/villas");
            VillaNumbers = new VillaNumberService(_httpClient, $@"{urlBase}/api/villanumbers");
            LocalUsers = new AuthService(_httpClient, $@"{urlBase}/api/users");

        }
    }
}