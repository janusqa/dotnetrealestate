using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.ApiService;
using RealEstate.UI.Models;
using RealEstate.Utility;

namespace RealEstate.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApiService _api;

        public HomeController(ILogger<HomeController> logger, IApiService api)
        {
            _logger = logger;
            _api = api;

        }

        public async Task<IActionResult> Index()
        {
            var villas = new List<VillaDto>();

            var response = await _api.Villas.GetAllAsync();
            var jsonData = Convert.ToString(response?.Result);
            if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
            {
                villas = JsonConvert.DeserializeObject<List<VillaDto>>(jsonData);
            }
            else
            {
                if (response?.ErrorMessages is not null)
                {
                    foreach (var message in response.ErrorMessages)
                    {
                        Console.WriteLine(message);
                    }
                }
            }

            return View(villas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
