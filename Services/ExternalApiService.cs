using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OrderBook.Models;

namespace OrderBook.Services
{
  public class ExternalApiService : IExternalApiService
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ExternalApiService(HttpClient httpClient, IConfiguration configuration)
    {
      _httpClient = httpClient;
      _configuration = configuration;
    }

    public async Task<OrderBookData> GetOrderBookDataAsync()
    {
      try
      {
        var apiUrl = _configuration["ExternalApi:BaseUrl"] + "order_book/" + "btceur";
        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OrderBookData>(content);
      }
      catch (Exception ex)
      {
        // Log the exception here
        throw;
      }
    }
  }
}