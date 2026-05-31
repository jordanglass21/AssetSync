using Microsoft.EntityFrameworkCore;
using AssetSync.Api.Data;
using AssetSync.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                builder.Configuration["AllowedOrigins"] ?? ""
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Tell app to use AppDbCOnext and SQLite using the connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register HttpClient so the service can make network calls
builder.Services.AddHttpClient<RealLegacyDataService>();

// This is where we wire up the dependency injection
builder.Services.AddScoped<ILegacyDataService, RealLegacyDataService>();

builder.Services.AddScoped<ReconciliationService>();

// Add services to the container
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("DevPolicy");
app.MapControllers();

app.Run();
