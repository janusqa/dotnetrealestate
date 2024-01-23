using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.ApiService;
using RealEstate.UI.Models;

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

            var response = await _api.Villas.GetAllAsync();
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

        public async Task<IActionResult> Create()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVillaDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _api.Villas.PostAsync(dto);
                var jsonData = Convert.ToString(response?.Result);
                if (response is not null && jsonData is not null)
                {
                    if (response.IsSuccess)
                    {
                        return RedirectToAction(nameof(Index));
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
            return View(dto);
        }

        public async Task<IActionResult> Update(int? entityId)
        {
            if (entityId is not null && entityId > 0)
            {
                var response = await _api.Villas.GetAsync(entityId.Value);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateVillaDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _api.Villas.PutAsync(dto.Id, dto);
                // var jsonData = Convert.ToString(response?.Result);
                if (response is not null)
                {
                    if (response.IsSuccess)
                    {
                        return RedirectToAction(nameof(Index), "Villa");
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
            return View(dto);
        }


        #region API CALLS
        [HttpDelete]
        [Route("Customer/Villa/Delete/{entityId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int entityId)
        {
            Console.WriteLine(entityId);
            // if (entityId is )
            // {
            var response = await _api.Villas.DeleteAsync(entityId);
            if (response is not null) return Ok(response);
            // }
            return Ok(new ApiResponse { IsSuccess = false, ErrorMessages = ["Oops, Something went wrong"], StatusCode = System.Net.HttpStatusCode.InternalServerError });
        }
        #endregion
    }
}