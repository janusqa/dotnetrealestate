using RealEstate.Dto;

namespace RealEstate.UI.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(AccessTokenDto accessTokenDto);
        AccessTokenDto? GetToken();
        void ClearToken();
        string? GetXsrfToken();
    }
}