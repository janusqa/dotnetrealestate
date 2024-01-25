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
        public const string ApiVersion = "v1";

    }
}