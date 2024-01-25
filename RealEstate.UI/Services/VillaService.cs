using RealEstate.Dto;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;

namespace RealEstate.UI.Services
{
    public class VillaService : BaseService<ApiResponse>, IVillaService
    {
        public VillaService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor, string url)
            : base(httpClient, httpContextAccessor, url)
        {

        }

    }
}