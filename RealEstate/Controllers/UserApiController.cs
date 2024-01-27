using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Dto;
using RealEstate.Models.Api;

namespace RealEstate.Controllers.v1
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

                    if (result is null)
                        return new ObjectResult(new ApiResponse { ErrorMessages = ["Invalid credentials"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };

                    return Ok(new ApiResponse { Result = result, IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
                }
                else
                {
                    return BadRequest(new ApiResponse { ErrorMessages = ["A valid email, and password is required to sign in"], StatusCode = System.Net.HttpStatusCode.BadRequest });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
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

                    if (result is null)
                        return new ObjectResult(new ApiResponse { ErrorMessages = ["Registration failed"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };

                    return Ok(new ApiResponse { Result = result, IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
                }
                else
                {
                    return BadRequest(new ApiResponse { ErrorMessages = ["Please check your information, and try again"], StatusCode = System.Net.HttpStatusCode.BadRequest });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}