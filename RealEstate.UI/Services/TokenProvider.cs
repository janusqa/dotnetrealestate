using RealEstate.Dto;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class TokenProvider : ITokenProvider
    {

        private readonly IHttpContextAccessor _hca;

        public TokenProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }

        public void ClearToken()
        {
            _hca.HttpContext?.Response.Cookies.Delete(SD.JwtAccessToken);
        }

        public TokenDto? GetToken()
        {
            try
            {
                string? token = null;
                bool? hasAccessToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.JwtAccessToken, out token);
                return hasAccessToken is not null
                        && hasAccessToken.Value
                        && token is not null
                        ? new TokenDto(AccessToken: token) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetToken(TokenDto tokenDto)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
            _hca.HttpContext?.Response.Cookies.Append(SD.JwtAccessToken, tokenDto.AccessToken, cookieOptions);
        }
    }
}