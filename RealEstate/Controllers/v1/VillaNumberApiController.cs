using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Models.Api;
using RealEstate.Models.Domain;
using RealEstate.Dto;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace RealEstate.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/villanumbers")]  // hard coded route name
    [ApiVersion("1.0")]

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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            try
            {
                var villaNumbers = (await _uow.VillaNumbers.SqlQueryAsync<VillaNumberWithIncluded>($@"
                    SELECT 
                        v.*,
                        vn.VillaNo,
                        vn.SpecialDetails,
                        vn.Id As VnId
                    FROM dbo.VillaNumbers vn INNER JOIN dbo.Villas v on (vn.VillaId = v.Id)
                ", []))
                .Select(vn =>
                    new VillaNumber
                    {
                        Id = vn.VnId,
                        SpecialDetails = vn.SpecialDetails,
                        VillaNo = vn.VillaNo,
                        VillaId = vn.Id,
                        Villa = new Villa
                        {
                            Id = vn.Id,
                            Name = vn.Name,
                            Details = vn.Details,
                            Rate = vn.Rate,
                            Sqft = vn.Sqft,
                            Occupancy = vn.Occupancy,
                            ImageUrl = vn.ImageUrl,
                            Amenity = vn.Amenity
                        }
                    }
                )
                .Select(vn => vn.ToDto()).ToList();

                return Ok(new ApiResponse { IsSuccess = true, Result = villaNumbers, StatusCode = System.Net.HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("{entityId:int}", Name = "GetVillaNumber")] // indicates that this endpoint expects an entityId
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Get(int entityId)
        {
            // lets do some simple validation
            if (entityId == 0) return BadRequest(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest });

            try
            {
                var villaNumber = (await _uow.VillaNumbers.SqlQueryAsync<VillaNumberWithIncluded>($@"
                   SELECT 
                        v.*,
                        vn.VillaNo,
                        vn.SpecialDetails,
                        vn.Id As VnId
                    FROM dbo.VillaNumbers vn INNER JOIN dbo.Villas v on (vn.VillaId = v.Id)
                    WHERE VillaNo = @Id
                ", [new SqlParameter("Id", entityId)]))
                .Select(vn =>
                    new VillaNumber
                    {
                        Id = vn.VnId,
                        SpecialDetails = vn.SpecialDetails,
                        VillaNo = vn.VillaNo,
                        VillaId = vn.Id,
                        Villa = new Villa
                        {
                            Id = vn.Id,
                            Name = vn.Name,
                            Details = vn.Details,
                            Rate = vn.Rate,
                            Sqft = vn.Sqft,
                            Occupancy = vn.Occupancy,
                            ImageUrl = vn.ImageUrl,
                            Amenity = vn.Amenity
                        }
                    }
                )
                .FirstOrDefault()?.ToDto();

                if (villaNumber is null) return NotFound(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.NotFound });

                return Ok(new ApiResponse { IsSuccess = true, Result = villaNumber, StatusCode = System.Net.HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }


        [HttpPost] // indicates that this endpoint expects an entityId
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Post([FromBody] CreateVillaNumberDto villaNumberDto)
        {

            // be careful with modelstate when debugging
            // it is checked before method is executed so breakpoing may not be triggered!
            if (!ModelState.IsValid) return BadRequest(new ApiResponse { IsSuccess = false, Result = ModelState, StatusCode = System.Net.HttpStatusCode.BadRequest });

            // lets do some simple validation
            if (villaNumberDto is null) return BadRequest(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest });

            var transaction = _uow.Transaction();
            try
            {
                var Id = (await _uow.VillaNumbers.SqlQueryAsync<int>($@"
                INSERT INTO dbo.VillaNumbers 
                    (VillaNo, VillaId, SpecialDetails)
                OUTPUT INSERTED.VillaNo
                    VALUES (@VillaNo, @VillaId, @SpecialDetails)
                ", [
                    new SqlParameter("VillaNo", villaNumberDto.VillaNo),
                    new SqlParameter("VillaId", villaNumberDto.VillaId),
                    new SqlParameter("SpecialDetails", villaNumberDto.SpecialDetails ?? (object)DBNull.Value)
                ])).FirstOrDefault();

                if (Id == 0) return BadRequest(new ApiResponse { IsSuccess = false, ErrorMessages = ["Invalid data provided"], StatusCode = System.Net.HttpStatusCode.BadRequest });

                transaction.Commit();

                return CreatedAtRoute("GetVillaNumber", new { entityId = Id }, new ApiResponse { IsSuccess = true, Result = villaNumberDto, StatusCode = System.Net.HttpStatusCode.Created });
            }
            catch (SqlException ex)
            {
                string message = ex.Errors[0].Number switch
                {
                    2627 => "Villa number already exists",
                    _ => ex.Message,
                };
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [message], StatusCode = System.Net.HttpStatusCode.BadRequest }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpDelete("{entityId:int}")] // indicates that this endpoint expects an entityId
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Delete(int entityId) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest });

            try
            {
                await _uow.VillaNumbers.ExecuteSqlAsync($@"
                DELETE FROM dbo.VillaNumbers WHERE VillaNo = @Id
                ", [new SqlParameter("Id", entityId)]);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            return Ok(new ApiResponse { IsSuccess = true, StatusCode = System.Net.HttpStatusCode.NoContent });
        }

        [HttpPut("{entityId:int}")] // indicates that this endpoint expects an entityId
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int entityId, [FromBody] UpdateVillaNumberDto villaNumberDto) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest, ErrorMessages = ["Invalid Entity Id"] });
            if (villaNumberDto is null || villaNumberDto.VillaNo != entityId) return BadRequest(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest, ErrorMessages = ["Invalid Entity Id"] });

            try
            {
                await _uow.VillaNumbers.ExecuteSqlAsync($@"
                    UPDATE dbo.VillaNumbers 
                    SET
                        VillaId = @VillaId,
                        SpecialDetails = @SpecialDetails
                    WHERE VillaNo = @Id
                ", [
                    new SqlParameter("Id", villaNumberDto.VillaNo),
                    new SqlParameter("VillaId", villaNumberDto.VillaId),
                    new SqlParameter("SpecialDetails", villaNumberDto?.SpecialDetails ?? (object)DBNull.Value)
                ]);
            }
            catch (SqlException ex)
            {
                string message = ex.Errors[0].Number switch
                {
                    547 => "Villa does not exist",
                    _ => ex.Message,
                };

                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            return Ok(new ApiResponse { IsSuccess = true, StatusCode = System.Net.HttpStatusCode.NoContent });
        }
    }
}