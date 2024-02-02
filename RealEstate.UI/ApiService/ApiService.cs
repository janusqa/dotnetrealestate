using RealEstate.UI.Services;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.ApiService
{
    public class ApiService : IApiService
    {
        public IVillaService Villas { get; init; }
        public IVillaNumberService VillaNumbers { get; init; }
        public IAuthService LocalUsers { get; init; }
        public IAuthService ApplicationUsers { get; init; }


        public ApiService(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpAccessor,
            ITokenProvider tokenProvider,
            IConfiguration configuration,
            IApiMessageRequestBuilder messageBuilder
        )
        {
            var urlBase = configuration.GetValue<string>("ServiceUrls:VillaApi");

            Villas = new VillaService(
                httpClient,
                httpAccessor,
                tokenProvider,
                messageBuilder,
                $@"{urlBase}/api/{SD.ApiVersion}/villas"
            );

            VillaNumbers = new VillaNumberService(
                httpClient,
                httpAccessor,
                tokenProvider,
                messageBuilder,
                $@"{urlBase}/api/{SD.ApiVersion}/villanumbers"
            );

            LocalUsers = new AuthService(
                httpClient,
                httpAccessor,
                tokenProvider,
                messageBuilder,
                $@"{urlBase}/api/{SD.ApiVersion}/users"
            );

            ApplicationUsers = new AuthService(
                httpClient,
                httpAccessor,
                tokenProvider,
                messageBuilder,
                $@"{urlBase}/api/{SD.ApiVersion}/users"
            );

        }
    }
}