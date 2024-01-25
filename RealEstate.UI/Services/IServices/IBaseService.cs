using RealEstate.UI.Models;

namespace RealEstate.UI.Services.IServices
{
    public interface IBaseService<T> where T : class
    {
        // Task<T?> RequestAsync(ApiRequest apiRequest);
        Task<T?> PostAsync<U>(U dto, string? token);
        Task<T?> PutAsync<U>(int entityId, U dto, string? token);
        Task<T?> DeleteAsync(int entityId, string? token);
        Task<T?> GetAsync(int entityI, string? token);
        Task<T?> GetAllAsync(string? token);
    }
}