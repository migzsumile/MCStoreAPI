using MCStore.Business.IServices;
using MCStore.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        // GET api/reports/sales?startDate=2026-01-01&endDate=2026-06-30
        [HttpGet("sales")]
        public async Task<ActionResult<SalesReportDTO>> GetSalesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var vm = await _reportsService.GetSalesReportAsync(startDate, endDate);
            return Ok(vm);
        }

        // GET api/reports/stockinsights?startDate=2026-01-01&endDate=2026-06-30
        [HttpGet("stockinsights")]
        public async Task<ActionResult<StockInsightsDTO>> GetStockInsights([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var vm = await _reportsService.GetStockInsightsAsync(startDate, endDate);
            return Ok(vm);
        }
    }
}
