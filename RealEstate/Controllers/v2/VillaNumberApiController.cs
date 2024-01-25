using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Models.Api;
using RealEstate.Models.Domain;
using RealEstate.Dto;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace RealEstate.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/villanumbers")]  // hard coded route name
    [ApiVersion("2.0")]

    public class VillaNumberApiController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<VillaNumberApiController> _logger;

        public VillaNumberApiController(IUnitOfWork uow, ILogger<VillaNumberApiController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            await Task.CompletedTask;
            return Ok(new ApiResponse { Result = new string[] { "value1", "value2" }, IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
        }
    }
}