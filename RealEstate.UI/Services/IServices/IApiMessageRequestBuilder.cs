using RealEstate.UI.Models;

namespace RealEstate.UI.Services.IServices
{
    public interface IApiMessageRequestBuilder
    {
        HttpRequestMessage Build(ApiRequest apiRequest);
    }
}