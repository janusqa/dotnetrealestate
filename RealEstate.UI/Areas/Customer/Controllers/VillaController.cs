using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.ApiService;
using RealEstate.UI.Models;
using RealEstate.Utility;

namespace RealEstate.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class VillaController : Controller
    {
        private readonly IApiService _api;

        public VillaController(IApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var villas = new List<VillaDto>();

            var response = await _api.Villas.GetAllAsync(HttpContext.Session.GetString(SD.SessionToken));
            var jsonData = Convert.ToString(response?.Result);
            if (response is not null && jsonData is not null)
            {
                if (response.IsSuccess)
                {
                    villas = JsonConvert.DeserializeObject<List<VillaDto>>(jsonData);
                }
                else
                {
                    if (response.ErrorMessages is not null)
                    {
                        foreach (var message in response.ErrorMessages)
                        {
                            Console.WriteLine(message);
                        }
                    }
                }
            }

            return View(villas);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await Task.CompletedTask;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVillaDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _api.Villas.PostAsync(dto, HttpContext.Session.GetString(SD.SessionToken));
                var jsonData = Convert.ToString(response?.Result);
                if (response is not null && jsonData is not null)
                {
                    if (response.IsSuccess)
                    {
                        TempData["success"] = "Villa created successfully!";
                        return RedirectToAction(nameof(Index), "Villa");
                    }
                    else
                    {
                        if (response.ErrorMessages is not null)
                        {
                            foreach (var (message, idx) in response.ErrorMessages.Select((e, idx) => (e, idx)))
                            {
                                ModelState.AddModelError($"CreateError[{idx}]", message);
                            }
                        }
                    }
                }
            }
            return View(dto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int? entityId)
        {
            if (entityId is not null && entityId > 0)
            {
                var response = await _api.Villas.GetAsync(entityId.Value, HttpContext.Session.GetString(SD.SessionToken));
                var jsonData = Convert.ToString(response?.Result);

                if (response is not null && jsonData is not null)
                {
                    if (response.IsSuccess)
                    {
                        var villa = JsonConvert.DeserializeObject<VillaDto>(jsonData);
                        if (villa is not null) return View(villa.ToUpdateDto());
                        else return NotFound();
                    }
                    else
                    {
                        if (response.ErrorMessages is not null)
                        {
                            foreach (var message in response.ErrorMessages)
                            {
                                Console.WriteLine(message);
                            }
                        }
                    }
                }
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateVillaDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _api.Villas.PutAsync(dto.Id, dto, HttpContext.Session.GetString(SD.SessionToken));
                // var jsonData = Convert.ToString(response?.Result);
                if (response is not null)
                {
                    if (response.IsSuccess)
                    {
                        TempData["success"] = "Villa updated successfully!";
                        return RedirectToAction(nameof(Index), "Villa");
                    }
                    else
                    {
                        if (response.ErrorMessages is not null)
                        {
                            foreach (var (message, idx) in response.ErrorMessages.Select((e, idx) => (e, idx)))
                            {
                                ModelState.AddModelError($"UpdateError[{idx}]", message);
                            }
                        }
                    }
                }
            }
            return View(dto);
        }


        #region API CALLS
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("Customer/Villa/Delete/{entityId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int entityId)
        {
            try
            {
                var response = await _api.Villas.DeleteAsync(entityId, HttpContext.Session.GetString(SD.SessionToken));
                if (response is not null && response.IsSuccess)
                {
                    TempData["success"] = "Villa deleted successfully!";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return new ObjectResult(new ApiResponse { ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            TempData["error"] = "Oops, Something went wrong";
            return new ObjectResult(new ApiResponse { ErrorMessages = ["Oops, Something went wrong"], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
        }
        #endregion
    }
}