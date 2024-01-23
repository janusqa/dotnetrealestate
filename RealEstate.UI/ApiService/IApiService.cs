
using RealEstate.UI.Services.IServices;

namespace RealEstate.UI.ApiService
{
    public interface IApiService
    {
        IVillaService Villas { get; init; }
        IVillaNumberService VillaNumbers { get; init; }

    }
}