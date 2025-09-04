using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderBook.Data;
using OrderBook.Services;
using OrderBook.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();
builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<WebSocketService>();
builder.Services.AddLogging();
builder.Services.AddDbContext<OrderBookContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

builder.Services.AddSingleton<IOrderBookAuditServiceFactory, OrderBookAuditServiceFactory>();
builder.Services.AddScoped<IOrderBookAuditService, OrderBookAuditService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderBookHub>("/orderBookHub");

app.Run();
