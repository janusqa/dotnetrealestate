using RealEstate.Dto;

namespace RealEstate.UI.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(TokenDto tokenDto);
        TokenDto? GetToken();
        void ClearToken();
    }
}