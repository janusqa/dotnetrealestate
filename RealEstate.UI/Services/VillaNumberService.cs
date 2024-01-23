using RealEstate.Dto;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;

namespace RealEstate.UI.Services
{
    public class VillaNumberService : BaseService<ApiResponse>, IVillaNumberService
    {
        public VillaNumberService(IHttpClientFactory httpClient, string url) : base(httpClient, url)
        {

        }

    }
}