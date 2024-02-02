using Newtonsoft.Json;

namespace RealEstate.MiddleWare
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public CustomExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // if no exceptions encountered continue on to next middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // if exception encountered process it
                // note "ProcessException" is a method and its name
                // is chosen by us the dev
                await ProcessException(context, ex);
                // we can choose to proceed 
                // to next middleware after processing error in this 
                // middleware. A reason that we may want to stop is 
                // if we cannot connect to say a database which is 
                // critical to api. Therefore we stop and flag a
                // criticle error.
                //  await _next(context); 
            }
        }

        private async Task ProcessException(HttpContext context, Exception ex)
        {
            // NOTE THIS IS JUST THE CODE FROM "CustomExceptionExtension" Extension
            // We are resuing it here as this is JUST ANOTHER WAY of handling errors
            // That is, using middleware.
            // Note we removed checking Feature as we have direct access to the exception
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            if (_env.IsDevelopment())
            {
                // *** begin - we can have custom logic here for some custom error code
                if (ex is BadImageFormatException badImageException)
                {
                    await context.Response.WriteAsync(
                        JsonConvert.SerializeObject(new
                        {
                            StatusCode = 776,
                            ErrorMessage = "Hello from custom Middleware, Bad image format!!!", // can be any message we like
                            StackTrace = ex.StackTrace
                        })
                    );
                }
                // *** end - we can have custom logic here for some custom error code
                else
                {
                    // default response for development env
                    await context.Response.WriteAsync(
                        JsonConvert.SerializeObject(new
                        {
                            StatusCode = context.Response.StatusCode,
                            ErrorMessage = ex.Message, // can be any message we like
                            StackTrace = ex.StackTrace
                        })
                    );
                }
            }
            else
            {
                // default response for production env
                await context.Response.WriteAsync(
                     JsonConvert.SerializeObject(new
                     {
                         StatusCode = context.Response.StatusCode,
                         ErrorMessage = "Hello from program.cs error handling middleware", // can be any message we like
                     })
                );
            }

        }
    }
}