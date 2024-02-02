using Microsoft.AspNetCore.Authentication.Cookies;
using RealEstate.UI.ApiService;
using RealEstate.UI.Extensions;
using RealEstate.UI.Services;
using RealEstate.UI.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(f => f.Filters.Add(new RedirectOnUnauthorized()));

builder.Services.AddHttpClient<IApiService, ApiService>("RealEstateAPI")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        // !!! DISABLE IN PROD. THIS IS TO BYPASS CHECKING SSL CERT AUTH FOR DEV PURPOSES !!!
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return handler;
    });

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IApiMessageRequestBuilder, ApiMessageRequestBuilder>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

// add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    // we needed to set LoginPath as it was going to the razor page for
    // identity which we ar not using yet. Our login page is "Auth/Login"
    // "not /Identity/Login" OR "/Account/Login" 
    options.LoginPath = "/Customer/Auth/Login";
    options.AccessDeniedPath = "/Customer/Auth/AccessDenied";
    options.SlidingExpiration = true;
});

// enable sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//add authentication
app.UseAuthentication();

app.UseAuthorization();

// use sessions
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
