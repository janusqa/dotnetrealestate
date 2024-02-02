using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace RealEstate.ErrorHandling.Extensions
{
    public static class CustomExceptionExtension
    {
        public static void HandleError(this IApplicationBuilder app, bool isDevelopment)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    if (feature != null)
                    {
                        if (isDevelopment)
                        {
                            // *** begin - we can have custom logic here for some custom error code
                            if (feature.Error is BadImageFormatException badImageException)
                            {
                                await context.Response.WriteAsync(
                                    JsonConvert.SerializeObject(new
                                    {
                                        StatusCode = 776,
                                        ErrorMessage = "Hello from custom HandleError Extension, Bad image format!!!", // can be any message we like
                                        StackTrace = feature.Error.StackTrace
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
                                        ErrorMessage = feature.Error.Message, // can be any message we like
                                        StackTrace = feature.Error.StackTrace
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
                                     ErrorMessage = "Hello from program.cs exception handler", // can be any message we like
                                 })
                            );
                        }
                    }
                });
            });
        }
    }
}