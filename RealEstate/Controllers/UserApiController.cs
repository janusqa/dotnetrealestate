using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Dto;
using RealEstate.Models.Api;
using RealEstate.Models.Domain;
using RealEstate.Utility;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    // [ApiVersion("1.0")]
    [ApiVersionNeutral]
    public class UserApiController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UserApiController(IUnitOfWork uow)
        {
            _uow = uow;

        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] ApplicationUserLoginRequestDto request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _uow.ApplicationUsers.Login(request);
                    if (result?.AccessToken is not null && result?.RefreshToken is not null && result?.XsrfToken is not null)
                    {
                        var acccessTokenDto = result.ToAccessTokenDto();

                        // refresh
                        Response.Cookies.Append(
                            SD.ApiRrefreshTokenCookie,
                            result.RefreshToken,
                            new CookieOptions()
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Path = "/",
                                MaxAge = DateTime.UtcNow.AddMinutes(SD.ApiRefreshTokenExpiry) - DateTime.UtcNow
                            });

                        // xsrf
                        Response.Cookies.Append(
                            SD.ApiXsrfCookie,
                            result.XsrfToken,
                            new CookieOptions()
                            {
                                HttpOnly = false,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Path = "/",
                                MaxAge = DateTime.UtcNow.AddMinutes(SD.ApiAccessTokenExpiry) - DateTime.UtcNow
                            });

                        // usually we should just return acccess token in json, refresh token in a httpOnly cookie and xsrf in a regular cookie
                        // return Ok(new ApiResponse { IsSuccess = true, Result = acccessTokenDto, StatusCode = System.Net.HttpStatusCode.OK });

                        // BUT our front-end is an .net core mvc app so we need to do things differently.  We must
                        // return access, refresh and xsrf all in the json and let the mvc app set and clear cookies for browser
                        return Ok(new ApiResponse { IsSuccess = true, Result = result, StatusCode = System.Net.HttpStatusCode.OK });

                    }
                    return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = ["Invalid credentials"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    return BadRequest(new ApiResponse { IsSuccess = false, ErrorMessages = ["A valid email, and password is required to sign in"], StatusCode = System.Net.HttpStatusCode.BadRequest });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] CreateApplicationUserDto request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _uow.ApplicationUsers.Register(request);

                    if (result?.AccessToken is not null && result?.RefreshToken is not null && result?.XsrfToken is not null)
                    {
                        var acccessTokenDto = result.ToAccessTokenDto();

                        // refresh
                        Response.Cookies.Append(
                            SD.ApiRrefreshTokenCookie,
                            result.RefreshToken,
                            new CookieOptions()
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Path = "/",
                                MaxAge = DateTime.UtcNow.AddMinutes(SD.ApiRefreshTokenExpiry) - DateTime.UtcNow
                            });

                        // xsrf
                        Response.Cookies.Append(
                            SD.ApiXsrfCookie,
                            result.XsrfToken,
                            new CookieOptions()
                            {
                                HttpOnly = false,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Path = "/",
                                MaxAge = DateTime.UtcNow.AddMinutes(SD.ApiAccessTokenExpiry) - DateTime.UtcNow
                            });

                        // usually we should just return acccess token in json, refresh token in a httpOnly cookie and xsrf in a regular cookie
                        // return Ok(new ApiResponse { IsSuccess = true, Result = acccessTokenDto, StatusCode = System.Net.HttpStatusCode.OK });

                        // BUT our front-end is an .net core mvc app so we need to do things differently.  We must
                        // return access, refresh and xsrf all in the json and let the mvc app set and clear cookies for browser
                        return Ok(new ApiResponse { IsSuccess = true, Result = result, StatusCode = System.Net.HttpStatusCode.OK });
                    }
                    return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = ["Registration failed"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };

                }
                else
                {
                    return BadRequest(new ApiResponse { IsSuccess = false, ErrorMessages = ["Please check your information, and try again"], StatusCode = System.Net.HttpStatusCode.BadRequest });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize]
        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            try
            {
                if (User.Identity?.Name is not null)
                {
                    await _uow.ApplicationUsers.ExecuteSqlAsync($@"
                        UPDATE dbo.AspNetUsers 
                        SET
                            UserSecret = @UserSecret
                        WHERE (UserName = @UserName)
                        ", [
                        new SqlParameter("UserName", User.Identity.Name),
                    new SqlParameter("UserSecret", BcryptUtils.CreateSalt())
                    ]);

                    Response.Cookies.Delete(SD.ApiRrefreshTokenCookie);
                    Response.Cookies.Delete(SD.ApiXsrfCookie);
                    return Ok(new ApiResponse { IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
                }
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = ["Invalid user"], StatusCode = System.Net.HttpStatusCode.BadRequest }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize]
        [HttpGet("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> Refresh()
        {
            try
            {
                var userName = User.Identity?.Name;
                if (userName is not null)
                {
                    var result = await _uow.ApplicationUsers.Refresh(userName);
                    if (result?.AccessToken is not null && result?.XsrfToken is not null)
                    {
                        var acccessTokenDto = result.ToAccessTokenDto();

                        // xsrf
                        Response.Cookies.Append(
                            SD.ApiXsrfCookie,
                            result.XsrfToken,
                            new CookieOptions()
                            {
                                HttpOnly = false,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Path = "/",
                                MaxAge = DateTime.UtcNow.AddMinutes(SD.ApiAccessTokenExpiry) - DateTime.UtcNow
                            });

                        // usually we should just return acccess token in json, refresh token in a httpOnly cookie and xsrf in a regular cookie
                        // return Ok(new ApiResponse { IsSuccess = true, Result = acccessTokenDto, StatusCode = System.Net.HttpStatusCode.OK });

                        // BUT our front-end is an .net core mvc app so we need to do things differently.  We must
                        // return access, refresh and xsrf all in the json and let the mvc app set and clear cookies for browser
                        return Ok(new ApiResponse { IsSuccess = true, Result = result, StatusCode = System.Net.HttpStatusCode.OK });
                    }
                    return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = ["Invalid credentials"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    return BadRequest(new ApiResponse { IsSuccess = false, ErrorMessages = ["User claim not present"], StatusCode = System.Net.HttpStatusCode.BadRequest });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

    }
}