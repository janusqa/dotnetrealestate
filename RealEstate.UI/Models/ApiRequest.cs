using static RealEstate.Utility.SD;

namespace RealEstate.UI.Models
{
    public class ApiRequest
    {
        public ApiMethod ApiMethod { get; set; } = ApiMethod.GET;
        public required string Url { get; set; }
        public object? Data { get; set; }
        public string? Token { get; set; }
    }
}