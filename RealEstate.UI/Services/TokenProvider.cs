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
            _hca.HttpContext?.Response.Cookies.Delete(SD.ApiRrefreshTokenCookie);
            _hca.HttpContext?.Response.Cookies.Delete(SD.ApiXsrfCookie);
        }

        public AccessTokenDto? GetToken()
        {
            try
            {
                string? accessToken = null;
                bool? hasAccessToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.JwtAccessToken, out accessToken);
                return hasAccessToken is not null
                        && hasAccessToken.Value
                        && accessToken is not null
                        ? new AccessTokenDto(AccessToken: accessToken) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string? GetXsrfToken()
        {
            try
            {
                string? xsrfToken = null;
                bool? hasXsrfToken = _hca.HttpContext?.Request.Cookies.TryGetValue(SD.ApiXsrfCookie, out xsrfToken);
                return hasXsrfToken is not null
                        && hasXsrfToken.Value
                        && xsrfToken is not null
                        ? xsrfToken : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetToken(AccessTokenDto accessTokenDto)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
            _hca.HttpContext?.Response.Cookies.Append(SD.JwtAccessToken, accessTokenDto.AccessToken, cookieOptions);
        }
    }
}