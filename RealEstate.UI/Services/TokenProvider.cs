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
            _hca.HttpContext?.Response.Cookies.Delete(SD.JwtAccessTokenCookie);
            _hca.HttpContext?.Response.Cookies.Delete(SD.ApiRrefreshTokenCookie);
            _hca.HttpContext?.Response.Cookies.Delete(SD.ApiXsrfCookie);
        }

        public TokenDto? GetToken()
        {
            try
            {
                string? accessToken = null;
                string? refreshToken = null;
                string? xsrfToken = null;
                bool hasAccessToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.JwtAccessTokenCookie, out accessToken) ?? false;
                bool hasRefreshToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.ApiRrefreshTokenCookie, out refreshToken) ?? false;
                bool hasXsrfToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.ApiXsrfCookie, out xsrfToken) ?? false;
                return new TokenDto(AccessToken: accessToken, XsrfToken: xsrfToken, RefreshToken: refreshToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetToken(TokenDto tokenDto)
        {
            ClearToken();

            if (tokenDto.AccessToken is not null)
                _hca.HttpContext?.Response.Cookies.Append(
                    SD.JwtAccessTokenCookie,
                    tokenDto.AccessToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(SD.ApiAccessTokenExpiry),
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });

            if (tokenDto.XsrfToken is not null)
                _hca.HttpContext?.Response.Cookies.Append(
                    SD.ApiXsrfCookie,
                    tokenDto.XsrfToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(SD.ApiAccessTokenExpiry),
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });

            if (tokenDto.RefreshToken is not null)
                _hca.HttpContext?.Response.Cookies.Append(
                    SD.ApiRrefreshTokenCookie,
                    tokenDto.RefreshToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(SD.ApiRefreshTokenExpiry),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });
        }
    }
}