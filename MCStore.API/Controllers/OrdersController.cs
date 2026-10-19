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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("GetOrdersFiltered")]
        public async Task<IActionResult> GetOrdersFiltered([FromQuery] string? searchString, [FromQuery] DateTime? transactionDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting orders list with filters.");
                var orders = await _orderService.GetOrdersFilteredAsync(searchString, transactionDate, pageNumber, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders list");
                return StatusCode(500, "An internal error occurred while retrieving orders.");
            }
        }

        [HttpGet("GetOrdersInitial")]
        public async Task<IActionResult> GetOrdersInitial([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting initial orders list (no filters) with pagination.");
                var orders = await _orderService.GetOrdersFilteredAsync(null, null, pageNumber, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting initial orders list");
                return StatusCode(500, "An internal error occurred while retrieving orders.");
            }
        }

        // GET: api/<OrdersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("GetStockOrderByCodeNameAsync")]
        public async Task<IActionResult> GetStockOrderByCodeNameAsync(string searchString)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Getting all expired stocks.");
                var stock = await _orderService.GetStockOrderByCodeNameAsync(searchString);
                return Ok(stock);
            }
            catch (Exception)
            {
                throw new Exception("Error getting all expired stocks");
            }
        }

        [HttpGet("BankFee")]
        public async Task<IActionResult> BankFee(string productCode, double amount)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Getting bank fee.");
                var ordersProductIsDigital = await _orderService.BankFeeAsync(productCode, amount);
                return Ok(ordersProductIsDigital);
            }
            catch (Exception)
            {
                throw new Exception("Error getting bank fee");
            }
        }

        [HttpGet("GetTotalPriceByQuantity")]
        public async Task<IActionResult> GetTotalPriceByQuantity(string productCode, double quantity)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Getting total price by quantity.");
                var totalPrice = await _orderService.GetTotalPriceByQuantity(productCode, quantity);
                return Ok(totalPrice);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                //Insufficient stock available for allocation.
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total price by quantity for {ProductCode}", productCode);
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<IActionResult> Post(string? customerName, decimal totalAmount, [FromBody] OrdersListCreateDTO orders)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Adding orders.");

                var ordersList = orders.Orders.Select(o => new Orders
                {
                    StockId = o.StockId,
                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPrice,
                    TotalPrice = o.TotalPrice,
                    Comments = o.Comments,
                }).ToList();
                await _orderService.AddInvoiceAsync(ordersList, customerName, totalAmount);
                return Ok("Orders added succesfully");
            }
            catch (Exception ex)
            {

                throw new Exception("Error adding orders");
            }
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}
