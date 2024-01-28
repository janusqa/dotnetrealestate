using System.Security.Policy;
using RealEstate.Dto;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class VillaService : BaseService<ApiResponse>, IVillaService
    {
        public VillaService(IHttpClientFactory httpClient, ITokenProvider tokenProvider, string url)
            : base(httpClient, tokenProvider, url)
        {

        }
    }
}