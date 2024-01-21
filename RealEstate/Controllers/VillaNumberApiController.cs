using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Models.Api;
using RealEstate.Models.Domain;
using RealEstate.Models.Dto;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/villanumbers")]  // hard coded route name

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
        public async Task<ActionResult<APIResponse>> GetAll()
        {
            try
            {
                var villaNumbers = (await _uow.VillaNumbers.FromSqlAsync($@"
                SELECT * FROM dbo.VillaNumbers
                ", [])).Select(v => v.ToDto()).ToList();

                return Ok(new APIResponse { Result = villaNumbers, IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new APIResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("{entityId:int}", Name = "GetVillaNumber")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Get(int entityId)
        {
            // lets do some simple validation
            if (entityId == 0) return BadRequest(new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest });

            try
            {
                var villaNumber = (await _uow.VillaNumbers.FromSqlAsync($@"
                SELECT * FROM dbo.VillaNumbers WHERE VillaNo = @Id
                ", [new SqlParameter("Id", entityId)])).FirstOrDefault();

                if (villaNumber is null) return NotFound(new APIResponse { StatusCode = System.Net.HttpStatusCode.NotFound });

                return Ok(new APIResponse { Result = villaNumber.ToDto(), IsSuccess = true, StatusCode = System.Net.HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new APIResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Post([FromBody] CreateVillaNumberDto villaNumberDto)
        {

            // be careful with modelstate when debugging
            // it is checked before method is executed so breakpoing may not be triggered!
            if (!ModelState.IsValid) return BadRequest(new APIResponse { Result = ModelState, StatusCode = System.Net.HttpStatusCode.BadRequest });

            // lets do some simple validation
            if (villaNumberDto is null) return BadRequest(new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest });

            var transaction = _uow.Transaction();
            try
            {
                var Id = (await _uow.VillaNumbers.SqlQueryAsync<int>($@"
                INSERT INTO dbo.VillaNumbers 
                    (VillaNo, SpecialDetails)
                OUTPUT INSERTED.VillaNo
                    VALUES (@VillaNo, @SpecialDetails)
                ", [
                    new SqlParameter("VillaNo", villaNumberDto.VillaNo),
                    new SqlParameter("SpecialDetails", villaNumberDto.SpecialDetails)
                ])).FirstOrDefault();

                if (Id == 0) return BadRequest(new APIResponse { ErrorMessages = ["Invalid data provided"], StatusCode = System.Net.HttpStatusCode.BadRequest });

                transaction.Commit();

                return CreatedAtRoute("GetVillaNumber", new { entityId = Id }, new APIResponse { Result = villaNumberDto, IsSuccess = true, StatusCode = System.Net.HttpStatusCode.Created });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ObjectResult(new APIResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpDelete("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Delete(int entityId) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest(new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest });

            try
            {
                await _uow.VillaNumbers.ExecuteSqlAsync($@"
                DELETE FROM dbo.VillaNumbers WHERE VillaNo = @Id
                ", [new SqlParameter("Id", entityId)]);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new APIResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            return NoContent();
        }

        [HttpPut("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int entityId, [FromBody] UpdateVillaNumberDto villaNumberDto) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest(new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ErrorMessages = ["Invalid Entity Id"] });
            if (villaNumberDto is null || villaNumberDto.VillaNo != entityId) return BadRequest(new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, ErrorMessages = ["Invalid Entity Id"] });

            try
            {
                await _uow.VillaNumbers.ExecuteSqlAsync($@"
                UPDATE dbo.VillaNumbers 
                SET
                    SpecialDetails = @SpecialDetails
                WHERE VillaNo = @Id
                ", [
                    new SqlParameter("Id", villaNumberDto.VillaNo),
                new SqlParameter("SpecialDetails", villaNumberDto?.SpecialDetails)
                ]);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new APIResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }


            return NoContent();
        }
    }
}