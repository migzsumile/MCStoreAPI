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
    [Authorize]
    public class CashinCashoutFeeController : ControllerBase
    {
        private readonly ICashInCashOutFeeService _cashInCashOutFeeService;
        private readonly IProductService _productService;

        public CashinCashoutFeeController(ICashInCashOutFeeService cashInCashOutFeeService, IProductService productService)
        {
            _cashInCashOutFeeService = cashInCashOutFeeService;
            _productService = productService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var list = await _cashInCashOutFeeService.GetAllAsync(pageNumber, pageSize);
            var dtoItems = list.Items.Select(x => new CashInCashOutFeeDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                WalletBalanceId = x.WalletBalanceId ?? null,
                StartAmount = x.StartAmount,
                EndAmount = x.EndAmount,
                Fee = x.Fee,
                WalletName = x.WalletBalance?.WalletName,
                ProductName = x.Product?.ProductName,
                ProductCode = x.Product?.ProductCode,
                Operator = x.Operator
            }).ToList();

            var result = new PaginationResult<CashInCashOutFeeDto>(dtoItems, list.PageNumber, list.PageSize, list.TotalCount);
            return Ok(result);
        }

        // GET: api/CashInCashOutFee/GetAllLatestWalletBalancesAsync
        [HttpGet("GetAllLatestWalletBalancesAsync")]
        public async Task<IActionResult> GetAllLatestWalletBalancesAsync()
        {
            var result = await _cashInCashOutFeeService.GetAllLatestWalletBalancesAsync();
            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _cashInCashOutFeeService.GetByIdAsync(id);
                var dto = new CashInCashOutFeeDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    StartAmount = item.StartAmount,
                    EndAmount = item.EndAmount,
                    Fee = item.Fee,
                    ProductName = item.Product?.ProductName,
                    ProductCode = item.Product?.ProductCode
                };
                return Ok(dto);
            }
            catch (Exception e)
            {
                return NotFound(new { Error = e.Message });
            }
        }

        [HttpGet("GetByProductId")]
        public async Task<IActionResult> GetByProductId(int productId, int pageNumber = 1, int pageSize = 10)
        {
            var list = await _cashInCashOutFeeService.GetByProductIdAsync(productId, pageNumber, pageSize);
            var dtoItems = list.Items.Select(x => new CashInCashOutFeeDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                StartAmount = x.StartAmount,
                EndAmount = x.EndAmount,
                Fee = x.Fee,
                ProductName = x.Product?.ProductName,
                ProductCode = x.Product?.ProductCode
            }).ToList();

            var result = new PaginationResult<CashInCashOutFeeDto>(dtoItems, list.PageNumber, list.PageSize, list.TotalCount);
            return Ok(result);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string q, int pageNumber = 1, int pageSize = 10)
        {
            var list = await _cashInCashOutFeeService.SearchAsync(q, pageNumber, pageSize);
            var dtoItems = list.Items.Select(x => new CashInCashOutFeeDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                StartAmount = x.StartAmount,
                EndAmount = x.EndAmount,
                Fee = x.Fee,
                ProductName = x.Product?.ProductName,
                ProductCode = x.Product?.ProductCode
            }).ToList();

            var result = new PaginationResult<CashInCashOutFeeDto>(dtoItems, list.PageNumber, list.PageSize, list.TotalCount);
            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] List<CashInCashOutFeeCreateDto> items)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var models = new List<CashInCashOutFee>();
                foreach (var i in items)
                {
                    var product = await _productService.GetProductByProductCode(i.ProductCode);
                    if (product == null || product.Id <= 0)
                        return BadRequest(new { Error = $"Product with code '{i.ProductCode}' not found." });

                    models.Add(new CashInCashOutFee
                    {
                        ProductId = product.Id,
                        StartAmount = i.StartAmount,
                        EndAmount = i.EndAmount,
                        Fee = i.Fee,
                        WalletBalanceId = i.WalletBalanceId ?? null,
                        Operator = i.Operator
                    });
                }

                await _cashInCashOutFeeService.AddAsync(models);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(new { Error = e.Message });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CashInCashOutFeeUpdateDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.GetProductByProductCode(item.ProductCode);
                if (product == null || product.Id <= 0)
                    return BadRequest(new { Error = $"Product with code '{item.ProductCode}' not found." });

                var model = new CashInCashOutFee
                {
                    ProductId = product.Id,
                    StartAmount = item.StartAmount,
                    EndAmount = item.EndAmount,
                    Fee = item.Fee,
                    Operator = item.Operator,
                    WalletBalanceId = item.walletBalanceId
                };

                await _cashInCashOutFeeService.UpdateAsync(id, model);
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new { Error = e.Message });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _cashInCashOutFeeService.DeleteAsync(id);
                return Ok(ok);
            }
            catch (Exception e)
            {
                return BadRequest(new { Error = e.Message });
            }
        }
    }
}
