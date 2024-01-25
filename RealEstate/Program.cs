using RealEstate.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.DataAccess.Repository;
using RealEstate.DataAccess.DBInitilizer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// enable serilog
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel
//     .Verbose()
//     .WriteTo.File("log/relogs.txt", rollingInterval: RollingInterval.Day)
//     .CreateLogger();
// builder.Host.UseSerilog();

builder.Services.AddControllers().AddNewtonsoftJson();

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
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // *** BEGIN - THIS MUST MUST BE ADDED TO SUPPORT BEARER TOKENS
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "RealEstate Api v 1.0",
        Description = "RealEstate Management Api",
        TermsOfService = new Uri("https://localhost:7002/Customer/Home/Privacy"),
        Contact = new OpenApiContact
        {
            Name = "JanusQA",
            Url = new Uri("https://localhost:7002"),
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://localhost:7002"),
        }
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "RealEstate Api v 2.0",
        Description = "RealEstate Management Api",
        TermsOfService = new Uri("https://localhost:7002/Customer/Home/Privacy"),
        Contact = new OpenApiContact
        {
            Name = "JanusQA",
            Url = new Uri("https://localhost:7002"),
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://localhost:7002"),
        }
    });
    // *** END - THIS MUST MUST BE ADDED TO SUPPORT BEARER TOKENS
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDBInitilizer, DBInitilizer>();

var JwtAccessSecret = builder.Configuration.GetValue<string>("ApiSettings:JwtAccessSecret") ?? "";
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(auth =>
{
    auth.RequireHttpsMetadata = false;
    auth.SaveToken = true;
    auth.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAccessSecret)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstateApi_v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "RealEstateApi_v2");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // when using Identity or roll your own jwt based auth
app.UseAuthorization();

await SeedDatabase();

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