using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Dto;
using RealEstate.Models.Api;

namespace RealEstate.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
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
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LocalUserLoginRequestDto request)
        {
            try
            {
                var response = await _uow.LocalUsers.Login(request);

                if (response is null)
                    return new ObjectResult(new ApiResponse { ErrorMessages = ["Invalid credentials"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };

                return Ok(new ApiResponse { Result = response, IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
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
        public async Task<ActionResult<ApiResponse>> Register([FromBody] CreateLocalUserDto request)
        {
            try
            {
                var response = await _uow.LocalUsers.Register(request);

                if (response is null)
                    return new ObjectResult(new ApiResponse { ErrorMessages = ["Registration failed"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };

                return Ok(new ApiResponse { Result = response, IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}