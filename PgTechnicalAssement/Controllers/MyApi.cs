using Microsoft.AspNetCore.Mvc;
using PgTechnicalAssement.Services;
using PgTechnicalAssement.Models;

namespace PgTechnicalAssement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MyApi(TransformStockData transformStockData) : ControllerBase
    {
        private readonly TransformStockData _transformStockData = transformStockData;

        [HttpGet("{symbol}")]
        public async Task<ActionResult<List<MyApiOutput>>> GetStockData(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest(new { message = "Symbol parameter is required." });
            }

            try
            {
                var result = await _transformStockData.GetTransformedStockDataAsync(symbol);

                if (result == null || result.Count == 0)
                {
                    return NotFound(new { message = $"No stock data found for '{symbol}'." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
