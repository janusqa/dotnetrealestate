using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RealEstate.ErrorHandling.Filters.Exceptions
{
    public class CustomExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is FileNotFoundException)
            {
                context.Result = new ObjectResult("Exception Filter: File not found")
                {
                    StatusCode = 503, // random status code for testing purposes
                };
                context.ExceptionHandled = true;
            }
            // Note that ImageError not handled here so it will exit this
            // Filter and be handled by the ErrorHandlerController
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}