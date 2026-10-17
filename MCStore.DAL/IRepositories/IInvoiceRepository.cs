using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.IRepositories
{
    public interface IInvoiceRepository
    {
        Task<int> CreateInvoice(Invoice invoice);

        Task<List<InvoiceDTO>> ListInvoicesByAsync();

        Task<List<InvoiceDTO>> ListInvoicesByDateAsync(DateTime dateTime);

        Task VoidInvoiceAsync(int invoiceId);

        Task VoidOrderAsync(int orderId);

        Task<List<InvoiceOrdersDTO>> GetInvoiceOrdersAsync(int invoiceId);
    }
}
