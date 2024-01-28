using RealEstate.Utility;

namespace RealEstate.UI.Services.IServices
{
    public interface IBaseService<T> where T : class
    {
        // Task<T?> RequestAsync(ApiRequest apiRequest);
        Task<T?> PostAsync<U>(U dto, SD.ContentType contentType = SD.ContentType.Json);
        Task<T?> PutAsync<U>(int entityId, U dto, SD.ContentType contentType = SD.ContentType.Json);
        Task<T?> DeleteAsync(int entityId);
        Task<T?> GetAsync(int entityI);
        Task<T?> GetAllAsync();
    }
}