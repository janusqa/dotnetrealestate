using RealEstate.Utility;
using static RealEstate.Utility.SD;

namespace RealEstate.UI.Models
{
    public class ApiRequest
    {
        public ApiMethod ApiMethod { get; set; } = SD.ApiMethod.GET;
        public required string Url { get; set; }
        public object? Data { get; set; }
        public ContentType ContentType { get; set; } = SD.ContentType.Json;
    }
}