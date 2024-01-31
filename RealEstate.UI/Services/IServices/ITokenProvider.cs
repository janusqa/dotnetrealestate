using RealEstate.Dto;

namespace RealEstate.UI.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(TokenDto TokenDto);
        TokenDto? GetToken();
        void ClearToken();
    }
}