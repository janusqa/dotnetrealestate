
using System.Text;
using Newtonsoft.Json;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class ApiMessageRequestBuilder : IApiMessageRequestBuilder
    {
        public HttpRequestMessage Build(ApiRequest apiRequest)
        {
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

            return message;
        }
    }
}