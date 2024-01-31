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
        private readonly ITokenProvider _tokenProvider;
        private readonly string _url;

        public BaseService(
            IHttpClientFactory httpClient,
            ITokenProvider tokenProvider,
            string url
        )
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _url = url;
        }

        public async Task<T?> PostAsync<U>(U dto, SD.ContentType contentType)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.POST,
                    Data = dto,
                    Url = _url,
                    ContentType = contentType
                }
            );
        }

        public async Task<T?> PutAsync<U>(int entityId, U dto, SD.ContentType contentType)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.PUT,
                    Data = dto,
                    Url = $"{_url}/{entityId}",
                    ContentType = contentType
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

        protected async Task<T?> RequestAsync(ApiRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = _httpClient.CreateClient("RealEstateAPI");

                HttpRequestMessage message = new HttpRequestMessage();

                var contentType = apiRequest.ContentType switch
                {
                    SD.ContentType.MultiPartFormData => "*/*",
                    _ => "application/json"
                };
                message.Headers.Add("Accept", contentType);

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
                    if (apiRequest.ContentType == SD.ContentType.MultiPartFormData)
                    {
                        // form multi-part
                        var content = new MultipartFormDataContent();
                        foreach (var item in apiRequest.Data.GetType().GetProperties())
                        {
                            var value = item.GetValue(apiRequest.Data);
                            if (value is IFormFile)
                            {
                                var file = (IFormFile)value;
                                if (file is not null)
                                {
                                    content.Add(new StreamContent(file.OpenReadStream()), item.Name, file.FileName);
                                }
                            }
                            else
                            {
                                content.Add(new StringContent(value?.ToString() ?? ""), item.Name);
                            }
                        }
                        message.Content = content;
                    }
                    else
                    {
                        // json 
                        message.Content = new StringContent(
                            JsonConvert
                                .SerializeObject(
                                    apiRequest.Data),
                                    Encoding.UTF8,
                                    "application/json"
                                );
                    }
                }

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    if (token?.AccessToken is not null && token.XsrfToken is not null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token.XsrfToken);
                    }
                }

                HttpResponseMessage httpMessage = await client.SendAsync(message);
                var jsonContent = await httpMessage.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<T>(jsonContent);
                return apiResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse
                {
                    ErrorMessages = [ex.Message],
                    IsSuccess = false
                };
                var resJson = JsonConvert.SerializeObject(errorResponse);
                var apiResponse = JsonConvert.DeserializeObject<T>(resJson);
                return apiResponse;
            }
        }
    }
}