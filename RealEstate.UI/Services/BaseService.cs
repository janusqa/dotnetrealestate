using System.Text;
using Newtonsoft.Json;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class BaseService : IBaseService
    {
        public ApiResponse ResponseModel { get; set; }
        public IHttpClientFactory HttpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            ResponseModel = new ApiResponse();
            HttpClient = httpClient;
        }

        public async Task<T?> RequestAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = HttpClient.CreateClient("RealEstateAPI");
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

                HttpResponseMessage apiResponse = await client.SendAsync(message);
                var jsonData = await apiResponse.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(jsonData);
                return data;
            }
            catch (Exception ex)
            {
                var dto = new ApiResponse
                {
                    ErrorMessages = [Convert.ToString(ex.Message)],
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var ApiResponse = JsonConvert.DeserializeObject<T>(res);
                return ApiResponse;
            }
        }
    }
}