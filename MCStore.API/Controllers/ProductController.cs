using MCStore.Business.IServices;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace MCStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IConfiguration configuration)
        {
            _productService = productService;
            _logger = logger;
            _configuration = configuration;
        }

        [Route("GetSampleReturn")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSampleReturn()
        {
            try
            {
                //var appConnectionString = _configuration.GetConnectionString("MCStoreContext");

                //return Ok($"API in App Service is working with connection string: {appConnectionString}");

                return Ok("API in App Service is working.");
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        [Route("GetAllProducts")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Fetching from product table.");
                var p = await _productService.GetAllProductsAsync(pageNumber, pageSize);
                return Ok(p);
            }
            catch (Exception e)
            {
                var details = new List<string>();
                var inner = e;
                while (inner != null)
                {
                    details.Add(inner.Message);
                    inner = inner.InnerException;
                }


                return StatusCode(500, new
                {
                    Error = "Database error occurred while fetching products.",
                    ExceptionMessages = details
                });
            }
        }

        // GET api/<ProductController>/5
        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var p = await _productService.GetProductByIdAsync(id);
                return Ok(p);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("GetProductByProductCode")]
        public async Task<IActionResult> GetProductByProductCode(string productCode)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Getting products by productCode.");
                var p = await _productService.GetProductByProductCode(productCode);
                return Ok(p);
            }
            catch (DbException db)
            {
                throw new Exception($"DB Exception: {db.Message}");
            }
            catch (Exception e)
            {
                var error = "Error in GetProductByProductCode: " + e.Message;
                var trace = e.StackTrace;
                throw new Exception($"Message: {e.Message} \r\n StackTrace: {e.StackTrace}");
            }
        }

        [HttpGet("SearchProduct")]
        public async Task<IActionResult> SearchProductByCodeName(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var p = await _productService.SearchProductByCodeName(pageNumber, pageSize, searchString);
                return Ok(p);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // POST api/<ProductController>
        [Route("AddProduct")]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductListDTO p)
        {
            try
            {
                await _productService.AddProductAsync(p.Products);
                return Ok(true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product p)
        {
            try
            {
                await _productService.UpdateProductAsync(id, p);
                return Ok(p);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var d = await _productService.DeleteProductAsync(id);
                return Ok(d);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
