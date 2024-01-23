using RealEstate.UI.Models;

namespace RealEstate.UI.Services.IServices
{
    public interface IBaseService<T> where T : class
    {
        // Task<T?> RequestAsync(ApiRequest apiRequest);
        Task<T?> PostAsync<U>(U dto);
        Task<T?> PutAsync<U>(int entityId, U dto);
        Task<T?> DeleteAsync(int entityId);
        Task<T?> GetAsync(int entityId);
        Task<T?> GetAllAsync();
    }
}