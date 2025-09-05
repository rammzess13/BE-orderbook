using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OrderBook.Services;
using OrderBook.Models;

namespace OrderBook.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class OrderBookController : ControllerBase
  {
    private readonly IExternalApiService _externalApiService;

    public OrderBookController(IExternalApiService externalApiService)
    {
      _externalApiService = externalApiService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderBook()
    {
      var result = await _externalApiService.GetOrderBookDataAsync();
      return Ok(result);
    }
  }
}

