using RealEstate.Dto;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;

namespace RealEstate.UI.Services
{
    public class VillaNumberService : BaseService<ApiResponse>, IVillaNumberService
    {
        public VillaNumberService(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpAccessor,
            ITokenProvider tokenProvider,
            IApiMessageRequestBuilder messageBuilder,
            string url
        )
            : base(httpClient, httpAccessor, tokenProvider, messageBuilder, url)
        {

        }

    }
}