using MCStore.Business.IServices;
using MCStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice(Invoice invoice)
        {
            try
            {
                var invoiceId = await _invoiceService.CreateInvoice(invoice);
                return Ok(invoiceId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("ListInvoicesByAsync")]
        public async Task<IActionResult> ListInvoicesByAsync(int pageNumber, int pageSize)
        {
            try
            {
                var invoices = await _invoiceService.ListInvoicesByAsync(pageNumber, pageSize);
                return Ok(invoices);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("ListInvoicesByDateAsync")]
        public async Task<IActionResult> ListInvoicesByDateAsync(DateTime dateTime, int pageNumber, int pageSize)
        {
            try
            {
                var invoices = await _invoiceService.ListInvoicesByDateAsync(dateTime, pageNumber, pageSize);
                return Ok(invoices);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("VoidInvoiceAsync")]
        public async Task<IActionResult> VoidInvoiceAsync(int invoiceId)
        {
            try
            {
                await _invoiceService.VoidInvoiceAsync(invoiceId);
                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("VoidOrderAsync")]
        public async Task<IActionResult> VoidOrderAsync(int orderId)
        {
            try
            {
                await _invoiceService.VoidOrderAsync(orderId);
                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("GetInvoiceOrdersAsync")]
        public async Task<IActionResult> GetInvoiceOrdersAsync(int invoiceId)
        {
            try
            {
                var invoiceOrders = await _invoiceService.GetInvoiceOrdersAsync(invoiceId);
                return Ok(invoiceOrders);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
