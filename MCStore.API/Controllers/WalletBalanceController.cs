using MCStore.Business.IServices;
using MCStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class WalletBalanceController : ControllerBase
    {
        private readonly IWalletBalanceService _walletBalanceService;

        public WalletBalanceController(IWalletBalanceService walletBalanceService)
        {
            _walletBalanceService = walletBalanceService;
        }

        // GET: api/WalletBalances?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _walletBalanceService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // GET: api/WalletBalances/GetAllLatestAsync
        [HttpGet("GetAllLatestAsync")]
        public async Task<IActionResult> GetAllLatestAsync()
        {
            var result = await _walletBalanceService.GetAllLatestAsync();
            return Ok(result);
        }

        // GET: api/WalletBalances/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _walletBalanceService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/WalletBalances
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WalletBalance model)
        {
            if (model == null) return BadRequest();

            var created = await _walletBalanceService.AddAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/WalletBalances/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] WalletBalance model)
        {
            if (model == null || id != model.Id) return BadRequest();

            await _walletBalanceService.UpdateAsync(id, model);
            return NoContent();
        }

        // DELETE: api/WalletBalances/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _walletBalanceService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
