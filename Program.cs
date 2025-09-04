using Microsoft.EntityFrameworkCore;
using OrderBook.Data;
using OrderBook.Services;
using OrderBook.Hubs; // Add this if OrderBookHub is in the Hubs namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();
builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<WebSocketService>();
builder.Services.AddLogging();
builder.Services.AddDbContext<OrderBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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

public class OrderBookAuditService : IOrderBookAuditService, IDisposable
{
    public async Task LogOrderBookSnapshot(string snapshot)
    {
        // TODO: Implement logging logic here
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        // TODO: Implement dispose logic here if needed
    }
}
