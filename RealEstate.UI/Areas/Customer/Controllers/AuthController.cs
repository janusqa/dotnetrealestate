using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.ApiService;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : Controller
    {

        private readonly IApiService _api;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IApiService api, ITokenProvider tokenProvider)
        {
            _api = api;
            _tokenProvider = tokenProvider;
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
                        var jwt = JsonConvert.DeserializeObject<AccessTokenDto>(jsonData);
                        if (jwt is not null && jwt.AccessToken is not null)
                        {
                            // save the session so it can be automatically sent on each request
                            // NOTE THIS DOES NOT SEND TOKEN TO API. IT JUST SAVES TOKEN 
                            // ON CLIENT TO ALLOW UI TO REMEMBER WE ARE LOGGED IN.
                            // TO PASS TO API MODIFY methods in services (IBaseServices) to take 
                            // a token parameter

                            // Although we are receving a user object we do not want to receive claims
                            // from there as we dont really have to return a user object from api
                            // we should really be only returning a token. But hey this is a demo
                            // Let us now retrive the claims from the Token.

                            var jwtTokenHandler = new JsonWebTokenHandler();
                            var jwtToken = jwtTokenHandler.ReadJsonWebToken(jwt.AccessToken);

                            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                            try
                            {
                                // This is an example of how to read a claim from a token
                                // var uniqueNameClaim = jwtToken.Claims.First(c => c.Type == "unique_name").Value
                                identity.AddClaim(new Claim(ClaimTypes.Name, jwtToken.Claims.First(c => c.Type == "unique_name").Value));
                                identity.AddClaim(new Claim(ClaimTypes.Role, jwtToken.Claims.First(c => c.Type == "role").Value));
                                identity.AddClaim(new Claim("xsrf", jwtToken.Claims.First(c => c.Type == "xsrf").Value));
                                var principal = new ClaimsPrincipal(identity);
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                                _tokenProvider.SetToken(jwt);
                                return RedirectToAction(nameof(Index), "Home");
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine($"Jwt Error: {ex.Message}");
                                return RedirectToAction(nameof(AccessDenied), "Auth");
                            }
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
            var roleList = new List<SelectListItem> {
                new SelectListItem{Text=SD.Role_Admin, Value=SD.Role_Admin},
                new SelectListItem{Text=SD.Role_Customer, Value=SD.Role_Customer}
            };

            ViewBag.RoleList = roleList;

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
                        var jwt = JsonConvert.DeserializeObject<AccessTokenDto>(jsonData);
                        if (jwt is not null && jwt.AccessToken is not null)
                        {
                            _tokenProvider.SetToken(jwt);
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

            var roleList = new List<SelectListItem> {
                new SelectListItem{Text=SD.Role_Admin, Value=SD.Role_Admin},
                new SelectListItem{Text=SD.Role_Customer, Value=SD.Role_Customer}
            };

            ViewBag.RoleList = roleList;

            return View(dto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _api.ApplicationUsers.LogoutAsync();
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction(nameof(Login), "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}