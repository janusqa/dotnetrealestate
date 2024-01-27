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
        public const string SessionToken = "JwtToken";

        // API Versioning
        public const string ApiVersion = "v2";

        // Role Constants
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
    }
}