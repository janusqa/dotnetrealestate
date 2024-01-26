using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.ApiService;
using RealEstate.Utility;

namespace RealEstate.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : Controller
    {

        private readonly IApiService _api;
        public AuthController(IApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ApplicationUserLoginRequestDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _api.ApplicationUsers.LoginAsync(dto);
                var jsonData = Convert.ToString(response?.Result);
                if (response is not null && jsonData is not null)
                {
                    if (response.IsSuccess)
                    {
                        var user = JsonConvert.DeserializeObject<ApplicationUserLoginResponseDto>(jsonData);
                        if (user is not null && user.Token is not null)
                        {
                            // save the session so it can be automatically sent on each request
                            // NOTE THIS DOES NOT SEND TOKEN TO API. IT JUST SAVES TOKEN 
                            // ON CLIENT TO ALLOW UI TO REMEMBER WE ARE LOGGED IN.
                            // TO PASS TO API MODIFY methods in services (IBaseServices) to take 
                            // a token parameter
                            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                            identity.AddClaim(new Claim(ClaimTypes.Name, user.User.UserName));
                            identity.AddClaim(new Claim(ClaimTypes.Role, user.User.Role));
                            var principal = new ClaimsPrincipal(identity);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                            HttpContext.Session.SetString(SD.SessionToken, user.Token);
                            return RedirectToAction(nameof(Index), "Home");
                        }
                    }
                    else
                    {
                        if (response.ErrorMessages is not null)
                        {
                            ModelState.AddModelError("LoginError", string.Join(" | ", response.ErrorMessages));
                        }
                    }
                }
            }

            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateApplicationUserDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _api.ApplicationUsers.RegisterAsync(dto);
                var jsonData = Convert.ToString(response?.Result);
                if (response is not null && jsonData is not null)
                {
                    if (response.IsSuccess)
                    {
                        var user = JsonConvert.DeserializeObject<ApplicationUserLoginResponseDto>(jsonData);
                        if (user is not null && user.Token is not null)
                        {
                            HttpContext.Session.SetString(SD.SessionToken, user.Token);
                            return RedirectToAction(nameof(Index), "Home");
                        }
                    }
                    else
                    {
                        if (response.ErrorMessages is not null)
                        {
                            ModelState.AddModelError("LoginError", string.Join(" | ", response.ErrorMessages));
                        }
                    }
                }
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken, "");
            return RedirectToAction(nameof(Login), "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}