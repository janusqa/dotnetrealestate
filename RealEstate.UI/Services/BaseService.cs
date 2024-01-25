using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _url;

        public BaseService(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpContextAccessor,
            string url
        )
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _url = url;
        }

        public async Task<T?> PostAsync<U>(U dto)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.POST,
                    Data = dto,
                    Url = _url
                }
            );
        }

        public async Task<T?> PutAsync<U>(int entityId, U dto)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.PUT,
                    Data = dto,
                    Url = $"{_url}/{entityId}"
                }
            );
        }

        public async Task<T?> DeleteAsync(int entityId)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.DELETE,
                    Url = $"{_url}/{entityId}"
                }
            );
        }

        public async Task<T?> GetAllAsync()
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = _url
                }
            );
        }

        public async Task<T?> GetAsync(int entityId)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = $"{_url}/{entityId}"
                }
            );
        }

        protected async Task<T?> RequestAsync(ApiRequest apiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient("RealEstateAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.Method = apiRequest.ApiMethod switch
                {
                    SD.ApiMethod.POST => HttpMethod.Post,
                    SD.ApiMethod.PUT => HttpMethod.Put,
                    SD.ApiMethod.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };
                message.RequestUri = new Uri(apiRequest.Url);
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(
                        JsonConvert
                            .SerializeObject(
                                apiRequest.Data),
                                Encoding.UTF8,
                                "application/json"
                            );
                }

                var token = _httpContextAccessor.HttpContext?.Session.GetString(SD.SessionToken);
                if (token is not null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage apiResponse = await client.SendAsync(message);
                var jsonData = await apiResponse.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(jsonData);
                return data;
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse
                {
                    ErrorMessages = [Convert.ToString(ex.Message)],
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(errorResponse);
                var apiResponse = JsonConvert.DeserializeObject<T>(res);
                return apiResponse;
            }
        }
    }
}