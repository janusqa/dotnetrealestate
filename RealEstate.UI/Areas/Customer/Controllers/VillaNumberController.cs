using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.ApiService;
using RealEstate.UI.Models;
using RealEstate.UI.Models.ViewModels;
using RealEstate.Utility;

namespace RealEstate.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class VillaNumberController : Controller
    {
        private readonly IApiService _api;

        public VillaNumberController(IApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var villaNumbers = new List<VillaNumberDto>();

            var response = await _api.VillaNumbers.GetAllAsync();
            var jsonData = Convert.ToString(response?.Result);
            if (response is not null && jsonData is not null)
            {
                if (response.IsSuccess)
                {
                    villaNumbers = JsonConvert.DeserializeObject<List<VillaNumberDto>>(jsonData);
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

            return View(villaNumbers);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await Task.CompletedTask;
            IEnumerable<SelectListItem> villaListSelect = [];

            var responseVillaList = await _api.Villas.GetAllAsync();
            var jsonDataVillaList = Convert.ToString(responseVillaList?.Result);
            if (responseVillaList is not null && jsonDataVillaList is not null)
            {
                if (responseVillaList.IsSuccess)
                {
                    var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonDataVillaList);
                    villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
                }
            }

            var vncv = new VillaNumberCreateView
            {
                Dto = new CreateVillaNumberDto(0, 0, ""),
                VillaList = villaListSelect
            };

            return View(vncv);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VillaNumberCreateView vncv)
        {
            if (ModelState.IsValid)
            {
                var dto = vncv.Dto;
                var response = await _api.VillaNumbers.PostAsync(dto);
                var jsonData = Convert.ToString(response?.Result);
                if (response is not null && jsonData is not null)
                {
                    if (response.IsSuccess)
                    {
                        TempData["success"] = "Villa number created successfully";
                        return RedirectToAction(nameof(Index), "VillaNumber");
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

            IEnumerable<SelectListItem> villaListSelect = [];

            var responseVillaList = await _api.Villas.GetAllAsync();
            var jsonDataVillaList = Convert.ToString(responseVillaList?.Result);
            if (responseVillaList is not null && jsonDataVillaList is not null)
            {
                if (responseVillaList.IsSuccess)
                {
                    var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonDataVillaList);
                    villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
                }
            }
            vncv.VillaList = villaListSelect;

            return View(vncv);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int? entityId)
        {
            if (entityId is not null && entityId > 0)
            {

                var response = await _api.VillaNumbers.GetAsync(entityId.Value);
                var jsonData = Convert.ToString(response?.Result);

                if (response is not null && jsonData is not null)
                {
                    if (response.IsSuccess)
                    {
                        var villaNumber = JsonConvert.DeserializeObject<VillaNumberDto>(jsonData);

                        if (villaNumber is not null)
                        {
                            IEnumerable<SelectListItem> villaListSelect = [];

                            var responseVillaList = await _api.Villas.GetAllAsync();
                            var jsonDataVillaList = Convert.ToString(responseVillaList?.Result);
                            if (responseVillaList is not null && jsonDataVillaList is not null)
                            {
                                if (responseVillaList.IsSuccess)
                                {
                                    var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonDataVillaList);
                                    villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
                                }
                            }

                            var villaNumberUpdateView = new VillaNumberUpdateView { Dto = villaNumber.ToUpdateDto(), VillaList = villaListSelect };
                            return View(villaNumberUpdateView);
                        }
                        else { return NotFound(); }
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

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VillaNumberUpdateView vnuv)
        {
            if (ModelState.IsValid)
            {
                var dto = vnuv.Dto;
                var response = await _api.VillaNumbers.PutAsync(dto.VillaNo, dto);
                // var jsonData = Convert.ToString(response?.Result);
                if (response is not null)
                {
                    if (response.IsSuccess)
                    {
                        TempData["success"] = "Villa number updated successfully";
                        return RedirectToAction(nameof(Index), "VillaNumber");
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

            IEnumerable<SelectListItem> villaListSelect = [];

            var responseVillaList = await _api.Villas.GetAllAsync();
            var jsonDataVillaList = Convert.ToString(responseVillaList?.Result);
            if (responseVillaList is not null && jsonDataVillaList is not null)
            {
                if (responseVillaList.IsSuccess)
                {
                    var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonDataVillaList);
                    villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
                }
            }
            vnuv.VillaList = villaListSelect;

            return View(vnuv);
        }


        #region API CALLS
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("Customer/VillaNumber/Delete/{entityId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int entityId)
        {
            try
            {
                var response = await _api.VillaNumbers.DeleteAsync(entityId);
                if (response is not null && response.IsSuccess)
                {
                    TempData["success"] = "Villa number deleted successfully!";
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