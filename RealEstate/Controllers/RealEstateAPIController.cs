using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.Models.Domain;
using RealEstate.Models.Dto;

namespace RealEstate.Controllers
{
    // NB: Derived from ControllerBase instead of Controller like in MVC
    // This is because we do not need access to Views which Controller Parent would provide
    // After all this is a API, i.e no UI just endpoints.
    [ApiController]
    [Route("api/RealEstateAPI/Villas")]  // hard coded route name
    // [controller] automagically gets the name of the controller from the class name.
    // Use with caution
    //[Route("api/[controller]")] 
    public class RealEstateAPIController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<RealEstateAPIController> _logger;

        public RealEstateAPIController(IUnitOfWork uow, ILogger<RealEstateAPIController> logger)
        {
            // Example of using the logger via dependancy injection
            // we can use the built in logger or up the ante with 
            // a third party like serilog
            // dotnet add RealEstate package Serilog.AspNetCore
            // dotnet add RealEstate package Serilog.Sinks.File (enables logging to files)
            // rigister this 3rd party as usual in programs.cs
            // ```
            //  Log.Logger = new LoggerConfiguration()
            //      .MinimumLevel
            //      .Verbose()
            //      .WriteTo.File("log/relogs.txt", rollingInterval: RollingInterval.Day)
            //      .CreateLogger();
            // builder.Host.UseSerilog();
            //```
            _uow = uow;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetAll()
        {
            _logger.LogInformation("Getting all villas!");

            var villas = (await _uow.Villas.FromSqlAsync($@"
                SELECT * FROM dbo.Villas
            ", [])).Select(v => v.ToDto()).ToList();

            return Ok(villas);
        }

        [HttpGet("{entityId:int}", Name = "Get")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> Get(int entityId)
        {
            // lets do some simple validation
            if (entityId == 0) return BadRequest();

            var villa = (await _uow.Villas.FromSqlAsync($@"
                SELECT * FROM dbo.Villas WHERE Id = @Id
            ", [new SqlParameter("Id", entityId)])).FirstOrDefault();

            if (villa is null) return NotFound();

            return Ok(villa.ToDto());
        }

        [HttpPost] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> Post([FromBody] CreateVillaDto villaDto)
        {

            // custom validations with modelstate
            var villaCount = (await _uow.Villas.SqlQueryAsync<int>($@"
                SELECT COUNT(Id) FROM dbo.Villas WHERE LOWER(Name) = LOWER(@Name)
            ", [new SqlParameter("Name", villaDto.Name)])).FirstOrDefault();

            if (villaCount > 0)
            {
                _logger.LogError("Duplicate insert detected!");

                ModelState.AddModelError("Duplicate", "Villa already exists!");
                return BadRequest(ModelState);
            }

            // be careful with modelstate when debugging
            // it is checked before method is executed so breakpoing may not be triggered!
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // lets do some simple validation
            if (villaDto is null) return BadRequest();

            var Id = (await _uow.Villas.SqlQueryAsync<int>($@"
                INSERT INTO dbo.Villas 
                    (Name, Details, ImageUrl, Occupancy, Rate, Sqft, Amenity)
                OUTPUT INSERTED.Id
                    VALUES (@Name, @Details, @ImageUrl, @Occupancy, @Rate, @Sqft, @Amenity)
            ", [
                    new SqlParameter("Name", villaDto.Name),
                    new SqlParameter("Details", villaDto.Details),
                    new SqlParameter("ImageUrl", villaDto.ImageUrl),
                    new SqlParameter("Occupancy", villaDto.Occupancy),
                    new SqlParameter("Rate", villaDto.Rate),
                    new SqlParameter("Sqft", villaDto.Sqft),
                    new SqlParameter("Amenity", villaDto.Amenity),
            ])).FirstOrDefault();

            if (Id == 0) return StatusCode(StatusCodes.Status500InternalServerError);

            return CreatedAtRoute(nameof(Get), new { entityId = Id }, villaDto);
        }

        [HttpDelete("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int entityId) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest();

            await _uow.Villas.ExecuteSqlAsync($@"
                DELETE FROM dbo.Villas WHERE Id = @Id
            ", [new SqlParameter("Id", entityId)]);

            return NoContent();
        }

        [HttpPut("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int entityId, [FromBody] UpdateVillaDto villaDto) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest();
            if (villaDto is null || villaDto.Id != entityId) return BadRequest();

            await _uow.Villas.ExecuteSqlAsync($@"
                UPDATE dbo.Villas 
                SET
                    Name = @Name,
                    Details = @Details,
                    ImageUrl = @ImageUrl,
                    Occupancy = @Occupancy,
                    Rate = @Rate,
                    Sqft = @Sqft,
                    Amenity = @Amenity
                WHERE Id = @Id
            ", [
                new SqlParameter("Id", entityId),
                new SqlParameter("Name", villaDto.Name),
                new SqlParameter("Details", villaDto.Details),
                new SqlParameter("ImageUrl", villaDto.ImageUrl),
                new SqlParameter("Occupancy", villaDto.Occupancy),
                new SqlParameter("Rate", villaDto.Rate),
                new SqlParameter("Sqft", villaDto.Sqft),
                new SqlParameter("Amenity", villaDto.Amenity),
            ]);

            return NoContent();
        }

        // FOR DEMO PURPOSES to demostrate patch.  Better to just use PUT in most cases
        // To support patch request must install two packages and the configure
        // one of them (NewtonsoftJson) in program.cs
        // 1. dotnet add RealEstate package Microsoft.AspNetCore.JsonPatch
        // 2. dotnet add RealEstate package Microsoft.AspNetCore.Mvc.NewtonsoftJson
        // 3. in programs.cs add .AddNewtonsoftJson() to builder.Services.AddControllers()
        // payload sent looks like ...
        // [
        //   {
        //     "path": "/name",
        //     "op": "replace",
        //     "value": "Turrents syn"
        //   }
        // ]
        // It represents a list of operations to perform on the DTO.
        // "path" is the attribute to be targetted.
        // "op" is what is to be done to the path, that is its an update/replace
        // "value" is the value to replace the original value is
        // https://jsonpatch.com/  <-- see for more informaation
        [HttpPatch("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch(int entityId, JsonPatchDocument<UpdateVillaDto> patchVillaDto)
        {
            if (entityId < 1) return BadRequest();
            if (patchVillaDto is null) return BadRequest();

            var villa = (await _uow.Villas.FromSqlAsync($@"
                SELECT * FROM dbo.Villas WHERE Id = @Id
            ", [new SqlParameter("Id", entityId)])).FirstOrDefault();

            if (villa is null) return NotFound();

            var upDateVillaDTO = villa.ToUpdateDto();

            patchVillaDto.ApplyTo(upDateVillaDTO, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _uow.Villas.ExecuteSqlAsync($@"
                UPDATE dbo.Villas 
                SET
                    Name = @Name,
                    Details = @Details,
                    ImageUrl = @ImageUrl,
                    Occupancy = @Occupancy,
                    Rate = @Rate,
                    Sqft = @Sqft,
                    Amenity = @Amenity
                WHERE Id = @Id
            ", [
                new SqlParameter("Id", entityId),
                new SqlParameter("Name", upDateVillaDTO.Name),
                new SqlParameter("Details", upDateVillaDTO.Details),
                new SqlParameter("ImageUrl", upDateVillaDTO.ImageUrl),
                new SqlParameter("Occupancy", upDateVillaDTO.Occupancy),
                new SqlParameter("Rate", upDateVillaDTO.Rate),
                new SqlParameter("Sqft", upDateVillaDTO.Sqft),
                new SqlParameter("Amenity", upDateVillaDTO.Amenity),
            ]);

            return NoContent();
        }

    }
}