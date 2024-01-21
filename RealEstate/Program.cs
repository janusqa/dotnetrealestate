using RealEstate.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using RealEstate.DataAccess.UnitOfWork.IUnitOfWork;
using RealEstate.DataAccess.Repository;
using RealEstate.DataAccess.DBInitilizer;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDBInitilizer, DBInitilizer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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