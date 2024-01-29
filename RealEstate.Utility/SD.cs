namespace RealEstate.Utility
{
    public static class SD
    {
        public enum ApiMethod
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        // sessions management
        public const string JwtAccessToken = "JwtAccessToken";

        // API Constants
        public const string ApiVersion = "v2";
        public const string ApiBaseUrl = "https://localhost:7001";
        public const string ApiRrefreshTokenCookie = "RETREFTOKEN";
        public const string ApiXsrfCookie = "XSRF-TOKEN";
        public const int ApiRefreshTokenExpiry = 5;
        public const int ApiAccessTokenExpiry = 1;

        // Role Constants
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public enum ContentType { Json, MultiPartFormData }
    }
}