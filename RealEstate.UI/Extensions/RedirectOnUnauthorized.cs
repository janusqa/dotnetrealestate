using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RealEstate.UI.Services.IServices;

namespace RealEstate.UI.Extensions
{
    public class RedirectOnUnauthorized : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is AuthenticationFailureException)
            {

                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}