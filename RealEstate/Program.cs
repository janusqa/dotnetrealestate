using RealEstate.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.DataAccess.Repository;
using RealEstate.DataAccess.DBInitilizer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Models.Identity;
using Microsoft.AspNetCore.Identity;
using RealEstate.Jwt;
using Microsoft.Extensions.Options;
using RealEstate;
using Swashbuckle.AspNetCore.SwaggerGen;
using RealEstate.ErrorHandling.Filters.Exceptions;
using RealEstate.ErrorHandling.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// enable serilog
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel
//     .Verbose()
//     .WriteTo.File("log/relogs.txt", rollingInterval: RollingInterval.Day)
//     .CreateLogger();
// builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(
        "Default30",
        new CacheProfile
        {
            Duration = 30
        }
    );
    options.Filters.Add<CustomExceptionFilter>();
}).ConfigureApiBehaviorOptions(options =>
    options.ClientErrorMapping[StatusCodes.Status500InternalServerError] = new ClientErrorData
    {
        Link = "https://fakelink.com/500" // this can be any link you like
    }
).AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// add custom services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDBInitilizer, DBInitilizer>();
builder.Services.AddScoped<ICustomJwtBearerHandler, CustomJwtBearerHandler>();

// add (jwt, could be other types of auth too) authentication
// var JwtAccessSecret = builder.Configuration.GetValue<string>("ApiSettings:JwtAccessSecret") ?? "";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddScheme<JwtBearerOptions, CustomJwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.IncludeErrorDetails = true;
});
// .AddJwtBearer(auth =>
// {
//     auth.RequireHttpsMetadata = false;
//     auth.SaveToken = true;
//     auth.TokenValidationParameters = new TokenValidationParameters
//     {
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAccessSecret)),
//         ValidateIssuerSigningKey = true,
//         ValidateLifetime = true,
//         ValidateIssuer = false,
//         ValidateAudience = false
//     };
// });

// add authorization
builder.Services.AddAuthorization();

// Enable caching
builder.Services.AddResponseCaching();

// enable API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AddApiVersionParametersWhenVersionNeutral = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfiguration>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//seed the db
await SeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "RealEstateApi_v2");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstateApi_v1");
    });
}

// enable exception handling
// 1. Enable this if using controller way of globally handling Errors
// app.UseExceptionHandler("/api/v2/errorhandler/processerror");
// 2. Enable this if using Extensions way of globallying handling Errors`
app.HandleError(app.Environment.IsDevelopment());

// enable static files in wwwroot
app.UseStaticFiles();

app.UseHttpsRedirection();

// enable caching
app.UseResponseCaching();

app.UseAuthentication(); // when using Identity or roll your own jwt based auth
app.UseAuthorization();

app.MapControllers();

app.Run();

async Task SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitilizer = scope.ServiceProvider.GetRequiredService<IDBInitilizer>();
        await dbInitilizer.Initilize();
    }
}