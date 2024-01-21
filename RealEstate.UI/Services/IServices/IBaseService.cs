using RealEstate.UI.Models;

namespace RealEstate.UI.Services.IServices
{
    public interface IBaseService
    {
        public ApiResponse ResponseModel { get; set; }
        Task<T?> RequestAsync<T>(ApiRequest apiRequest);

    }
}