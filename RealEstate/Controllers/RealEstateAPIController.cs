using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealEstate.DataAccess;
using RealEstate.Models.Domain;
using RealEstate.Models.Domain.Dto;

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
        private readonly ILogger<RealEstateAPIController> _logger;

        public RealEstateAPIController(ILogger<RealEstateAPIController> logger)
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
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetAll()
        {
            _logger.LogInformation("Getting all villas!");
            return Ok(DataStore.villaList);
        }

        [HttpGet("{entityId:int}", Name = "Get")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> Get(int entityId)
        {
            // lets do some simple validation
            if (entityId == 0) return BadRequest();

            var villa = DataStore.villaList.Where(v => v.Id == entityId).FirstOrDefault();

            if (villa is null) return NotFound();

            return Ok(villa);
        }

        [HttpPost] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> Post([FromBody] VillaDTO villaDTO)
        {

            // custom validations with modelstate
            if (DataStore.villaList.Any(v => v.Name.Equals(villaDTO.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                _logger.LogError("Duplicate insert detected!");
                ModelState.AddModelError("Duplicate", "Villa already exists!");
                return BadRequest(ModelState);
            }

            // be careful with modelstate when debugging
            // it is checked before method is executed so breakpoing may not be triggered!
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // lets do some simple validation
            if (villaDTO is null) return BadRequest();

            if (villaDTO.Id > 0) return StatusCode(StatusCodes.Status400BadRequest); // we can return status codes like this as well

            villaDTO.Id = DataStore.villaList.Max(v => v.Id) + 1;
            DataStore.villaList.Add(villaDTO);

            return CreatedAtRoute(nameof(Get), new { entityId = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int entityId) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest();

            var index = DataStore.villaList.FindIndex(v => v.Id == entityId);
            if (index != -1)
            {
                DataStore.villaList.RemoveAt(index);
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put(int entityId, [FromBody] VillaDTO villaDTO) // not returning a type so can use IActionResult as return type
        {
            if (entityId < 1) return BadRequest();
            if (villaDTO is null || villaDTO.Id != entityId) return BadRequest();

            var index = DataStore.villaList.FindIndex(v => v.Id == entityId);
            if (index != -1)
            {
                DataStore.villaList[index].Name = villaDTO.Name;
                DataStore.villaList[index].Sqft = villaDTO.Sqft;
                DataStore.villaList[index].Occupancy = villaDTO.Occupancy;
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

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
        public IActionResult Patch(int entityId, JsonPatchDocument<VillaDTO> partialVillDto)
        {
            if (entityId < 1) return BadRequest();
            if (partialVillDto is null) return BadRequest();

            var index = DataStore.villaList.FindIndex(v => v.Id == entityId);
            if (index != -1)
            {
                partialVillDto.ApplyTo(DataStore.villaList[index], ModelState);
                if (!ModelState.IsValid) return BadRequest(ModelState);
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}