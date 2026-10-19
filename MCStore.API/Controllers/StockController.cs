using MCStore.Business.IServices;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockService stockService, ILogger<StockController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        // GET: api/<StockController>/GetAllStocksAvailables
        [HttpGet("GetAllStocksAvailable")]
        public async Task<IActionResult> GetAllStocksAvailable(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Getting all available stocks.");
                var stocks = await _stockService.GetAllStocksAvailable(pageNumber, pageSize);
                return Ok(stocks);
            }
            catch (Exception)
            {
                throw new Exception("Error getting all available stocks");
            }

        }

        // GET api/<StockController>/GetAllStocksExpired
        [HttpGet("GetAllStocksExpired")]
        public async Task<IActionResult> GetAllStocksExpired(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Getting all expired stocks.");
                var stocks = await _stockService.GetAllStocksExpired(pageNumber, pageSize);
                return Ok(stocks);
            }
            catch (Exception)
            {
                throw new Exception("Error getting all expired stocks");
            }
        }

        // GET api/<StockController>/GetStocksByProductAsync
        [HttpGet("GetStocksByProductAsync")]
        public async Task<IActionResult> GetStocksByProductAsync(int pageNumber, int pageSize, string searchString)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Getting all expired stocks.");
                var stocks = await _stockService.GetStocksByProductAsync(pageNumber, pageSize, searchString);
                return Ok(stocks);
            }
            catch (Exception)
            {
                throw new Exception("Error getting all expired stocks");
            }
        }

        // POST api/<StockController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StocksList s)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Adding stocks.");
                await _stockService.AddStocksAsync(s.Stocks);
                return Ok("Stocks added succesfully");

            }
            catch (Exception)
            {

                throw new Exception("Error creating stock");
            }
        }

        // PUT api/<StockController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Stocks s)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Updating stocks.");
                await _stockService.UpdateStockAsync(id, s);

                return Ok("Stock updated succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock.");
                return BadRequest("Stock update failed");
            }
        }

        // DELETE api/<StockController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Deleting stocks.");
                var delete = await _stockService.DeleteStock(id);
                if (delete > 0)
                    return Ok("Stock deleted succesfully");
                return BadRequest("Stock delete failed");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
