using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using RealEstate.Dto;
using RealEstate.UI.Models;
using RealEstate.UI.Services.IServices;
using RealEstate.Utility;

namespace RealEstate.UI.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ITokenProvider _tokenProvider;
        private readonly IApiMessageRequestBuilder _messageBuilder;
        private readonly string _url;

        public BaseService(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpAccessor,
            ITokenProvider tokenProvider,
            IApiMessageRequestBuilder messageBuilder,
            string url
        )
        {
            _httpClient = httpClient;
            _httpAccessor = httpAccessor;
            _tokenProvider = tokenProvider;
            _messageBuilder = messageBuilder;
            _url = url;
        }

        public async Task<T?> PostAsync<U>(U dto, SD.ContentType contentType)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.POST,
                    Data = dto,
                    Url = _url,
                    ContentType = contentType
                }
            );
        }

        public async Task<T?> PutAsync<U>(int entityId, U dto, SD.ContentType contentType)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.PUT,
                    Data = dto,
                    Url = $"{_url}/{entityId}",
                    ContentType = contentType
                }
            );
        }

        public async Task<T?> DeleteAsync(int entityId)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.DELETE,
                    Url = $"{_url}/{entityId}"
                }
            );
        }

        public async Task<T?> GetAllAsync()
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = _url
                }
            );
        }

        public async Task<T?> GetAsync(int entityId)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = $"{_url}/{entityId}"
                }
            );
        }

        protected async Task<T?> RequestAsync(ApiRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = _httpClient.CreateClient("RealEstateAPI");

                var messageFactory = () => _messageBuilder.Build(apiRequest);

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    if (token?.AccessToken is not null && token.XsrfToken is not null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token.XsrfToken);
                    }
                }

                HttpResponseMessage httpResponseMessage = await client.SendAsync(messageFactory());
                // 1. If statuscode is authorized continure processing
                // 2. If statuscode is unauthorized and access token is expired. Refresh access token (using refresh token).

                if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var token = _tokenProvider.GetToken();

                    if (token?.RefreshToken is null)
                    {
                        var errorsDefault = "You are not authorized to perform this action. Please login.";
                        var jsonErrors = await httpResponseMessage.Content.ReadAsStringAsync();
                        if (jsonErrors is not null && !string.IsNullOrEmpty(jsonErrors))
                        {
                            var response = JsonConvert.DeserializeObject<ApiResponse>(jsonErrors);
                            jsonErrors = JsonConvert.SerializeObject(response?.ErrorMessages);
                            if (response is not null && !string.IsNullOrEmpty(jsonErrors))
                            {
                                var errorMessages = JsonConvert.DeserializeObject<List<string>>(jsonErrors);
                                if (errorMessages is not null && errorMessages.Count != 0)
                                {
                                    errorsDefault = string.Join(" | ", errorMessages);
                                }
                            }
                        }

                        throw new AuthenticationFailureException(errorsDefault);
                    }

                    var jwtTokenHandler = new JsonWebTokenHandler();
                    var refreshToken = jwtTokenHandler.ReadJsonWebToken(token.RefreshToken);
                    var expRefreshtoken = long.Parse(refreshToken.Claims.First(c => c.Type == "exp").Value);
                    var expRefreshTokenDate = DateTimeOffset.FromUnixTimeSeconds(expRefreshtoken).UtcDateTime;

                    var jwtAuthStatus = httpResponseMessage.Headers.WwwAuthenticate.ToString();

                    if (
                        !string.IsNullOrEmpty(token.RefreshToken) &&
                        expRefreshTokenDate >= DateTime.Now.ToUniversalTime() &&
                        (
                            jwtAuthStatus.Contains("token expired") ||
                            jwtAuthStatus.Trim().Equals("bearer", StringComparison.CurrentCultureIgnoreCase)
                        )
                    )
                    {
                        var refreshTokenDto = await RefreshTokenAsync(client, token.RefreshToken);
                        if (refreshTokenDto?.AccessToken is not null && refreshTokenDto.XsrfToken is not null)
                        {
                            var jwt = jwtTokenHandler.ReadJsonWebToken(refreshTokenDto.AccessToken);
                            var claims = new List<Claim> {
                                new Claim(ClaimTypes.Name, jwt.Claims.First(c => c.Type == "unique_name").Value),
                                new Claim(ClaimTypes.Role, jwt.Claims.First(c => c.Type == "role").Value),
                                new Claim("xsrf", jwt.Claims.First(c => c.Type == "xsrf").Value)
                            };
                            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                            if (_httpAccessor.HttpContext is not null)
                                await _httpAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                            _tokenProvider.SetToken(refreshTokenDto);

                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshTokenDto.AccessToken);
                            client.DefaultRequestHeaders.Remove("X-XSRF-TOKEN");
                            client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", refreshTokenDto.XsrfToken);

                            httpResponseMessage = await client.SendAsync(messageFactory());
                        }
                        else
                        {
                            throw new AuthenticationFailureException("You are not authorized to perform this action. Please login.");
                        }
                    }
                    else
                    {
                        throw new AuthenticationFailureException("You are not authorized to perform this action. Please login.");
                    }
                }

                var jsonData = await httpResponseMessage.Content.ReadAsStringAsync();
                if (jsonData is not null && !string.IsNullOrEmpty(jsonData))
                {
                    // Success (response could still be a bad request etc. though)
                    // this just indicates a valid response that is not a 401 Unauthorized
                    // Note we conrcretely deserialize to an ApIResponse. Seems to negate
                    // the benefits of having this class as a generic but se le vie.
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonData);


                    if (!httpResponseMessage.IsSuccessStatusCode && apiResponse is not null)
                    {
                        apiResponse.ErrorMessages = httpResponseMessage.StatusCode switch
                        {
                            HttpStatusCode.NotFound => [.. (apiResponse.ErrorMessages ?? []), "Not Found"],
                            HttpStatusCode.Forbidden => [.. (apiResponse.ErrorMessages ?? []), "Access Denied"],
                            HttpStatusCode.Unauthorized => [.. (apiResponse.ErrorMessages ?? []), "Unauthorized"],
                            HttpStatusCode.InternalServerError => [.. (apiResponse.ErrorMessages ?? []), "Internal Server Error"],
                            _ => ["Oops, something went wrong. Please try again later"]
                        };
                    }

                    if (apiResponse is not null)
                    {
                        apiResponse.StatusCode = httpResponseMessage.StatusCode;
                        jsonData = JsonConvert.SerializeObject(apiResponse);
                        var response = JsonConvert.DeserializeObject<T>(jsonData);
                        return response;
                    }
                    else
                    {
                        throw new Exception("Oops, something went wrong. Please try again later");
                    }
                }
                else
                {
                    throw new Exception("Oops, something went wrong. Please try again later");
                }
            }
            catch (AuthenticationFailureException)
            {
                if (_httpAccessor.HttpContext is not null) await _httpAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw;
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse
                {
                    ErrorMessages = [ex.Message],
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                var jsonError = JsonConvert.SerializeObject(errorResponse);
                var apiResponse = JsonConvert.DeserializeObject<T>(jsonError);
                return apiResponse;
            }
        }

        private async Task<TokenDto?> RefreshTokenAsync(HttpClient client, string refreshToken)
        {
            Uri url = new Uri(_url);
            string baseUrl = $"{url.Scheme}://{url.Host}:{url.Port}";
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri($"{baseUrl}/api/{SD.ApiVersion}/users/refresh");
            message.Method = HttpMethod.Get;
            message.Headers.Add("Cookie", $"{SD.ApiRrefreshTokenCookie}={refreshToken}");

            HttpResponseMessage httpResponseMessage = await client.SendAsync(message);
            var jsonData = await httpResponseMessage.Content.ReadAsStringAsync();
            if (jsonData is not null && !string.IsNullOrEmpty(jsonData))
            {
                var response = JsonConvert.DeserializeObject<ApiResponse>(jsonData);
                jsonData = JsonConvert.SerializeObject(response?.Result);
                if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
                {
                    var tokenDto = JsonConvert.DeserializeObject<TokenDto>(jsonData);
                    if (tokenDto?.AccessToken is not null && tokenDto.XsrfToken is not null)
                    {
                        return new TokenDto(
                            AccessToken: tokenDto.AccessToken,
                            XsrfToken: tokenDto.XsrfToken,
                            RefreshToken: refreshToken
                        );
                    }
                }
            }

            return null;
        }
    }
}