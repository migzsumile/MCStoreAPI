using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.IServices
{
    public interface IInvoiceService
    {
        Task<int> CreateInvoice(Invoice invoice);

        Task<PaginationResult<InvoiceDTO>> ListInvoicesByAsync(int pageNumber, int pageSize);

        Task<PaginationResult<InvoiceDTO>> ListInvoicesByDateAsync(DateTime dateTime, int pageNumber, int pageSize);

        Task VoidInvoiceAsync(int invoiceId);

        Task VoidOrderAsync(int orderId);

        Task<List<InvoiceOrdersDTO>> GetInvoiceOrdersAsync(int invoiceId);
    }
}
