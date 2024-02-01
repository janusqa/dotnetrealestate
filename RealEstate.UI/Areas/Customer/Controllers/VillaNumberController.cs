using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.ApiService;
using RealEstate.UI.Models;
using RealEstate.UI.Models.ViewModels;

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
            var jsonData = JsonConvert.SerializeObject(response?.Result);
            if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
            {
                villaNumbers = JsonConvert.DeserializeObject<List<VillaNumberDto>>(jsonData) ?? [];
            }

            return View(villaNumbers);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await Task.CompletedTask;
            IEnumerable<SelectListItem> villaListSelect = [];

            var response = await _api.Villas.GetAllAsync();
            var jsonData = JsonConvert.SerializeObject(response?.Result);
            if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
            {
                var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonData);
                villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
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
            ApiResponse? response = null;
            string? jsonData = null;

            if (ModelState.IsValid)
            {
                var dto = vncv.Dto;
                response = await _api.VillaNumbers.PostAsync(dto);
                // jsonData = JsonConvert.SerializeObject(response?.Result);
                if (response is not null && response.IsSuccess)
                {
                    TempData["success"] = "Villa number created successfully";
                    return RedirectToAction(nameof(Index), "VillaNumber");
                }
                else
                {
                    if (response?.ErrorMessages is not null)
                    {
                        foreach (var (message, idx) in response.ErrorMessages.Select((e, idx) => (e, idx)))
                        {
                            ModelState.AddModelError($"CreateError[{idx}]", message);
                        }
                    }
                }
            }

            IEnumerable<SelectListItem> villaListSelect = [];

            response = await _api.Villas.GetAllAsync();
            jsonData = JsonConvert.SerializeObject(response?.Result);
            if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
            {
                var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonData);
                villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
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
                var jsonData = JsonConvert.SerializeObject(response?.Result);

                if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
                {
                    var villaNumber = JsonConvert.DeserializeObject<VillaNumberDto>(jsonData);

                    if (villaNumber is not null)
                    {
                        IEnumerable<SelectListItem> villaListSelect = [];

                        var responseVillaList = await _api.Villas.GetAllAsync();
                        var jsonDataVillaList = JsonConvert.SerializeObject(responseVillaList?.Result);
                        if (responseVillaList is not null && responseVillaList.IsSuccess)
                        {
                            var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonDataVillaList);
                            villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
                        }

                        var villaNumberUpdateView = new VillaNumberUpdateView { Dto = villaNumber.ToUpdateDto(), VillaList = villaListSelect };
                        return View(villaNumberUpdateView);
                    }
                }
                else
                {
                    if (response?.ErrorMessages is not null)
                    {
                        foreach (var (message, idx) in response.ErrorMessages.Select((e, idx) => (e, idx)))
                        {
                            ModelState.AddModelError($"UpdateError[{idx}]", message);
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
            ApiResponse? response = null;

            if (ModelState.IsValid)
            {
                var dto = vnuv.Dto;
                response = await _api.VillaNumbers.PutAsync(dto.VillaNo, dto);
                // var jsonData = JsonConvert.SerializeObject(response?.Result);
                if (response is not null && response.IsSuccess)
                {
                    TempData["success"] = "Villa number updated successfully";
                    return RedirectToAction(nameof(Index), "VillaNumber");
                }
                else
                {
                    if (response?.ErrorMessages is not null)
                    {
                        foreach (var (message, idx) in response.ErrorMessages.Select((e, idx) => (e, idx)))
                        {
                            ModelState.AddModelError($"UpdateError[{idx}]", message);
                        }
                    }
                }

            }

            IEnumerable<SelectListItem> villaListSelect = [];

            response = await _api.Villas.GetAllAsync();
            var jsonData = JsonConvert.SerializeObject(response?.Result);
            if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
            {
                var villaList = JsonConvert.DeserializeObject<List<VillaDto>>(jsonData);
                villaListSelect = villaList?.Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }) ?? [];
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
                var errorMessages = response?.ErrorMessages is not null ? string.Join(" | ", response.ErrorMessages) : "Oops, Something went wrong";
                TempData["error"] = "Oops, Something went wrong";
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = ["Oops, Something went wrong"], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
        #endregion
    }
}