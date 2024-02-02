using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RealEstate.UI.Filters
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