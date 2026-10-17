using MCStore.Business.IServices;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<int> CreateInvoice(Invoice invoice)
        {
            return await _invoiceRepository.CreateInvoice(invoice);
        }

        public async Task<PaginationResult<InvoiceDTO>> ListInvoicesByAsync(int pageNumber, int pageSize)
        {
            var invoices = await _invoiceRepository.ListInvoicesByAsync();

            var totalCount = invoices.Count;
            var pagedInvoices = invoices
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationResult<InvoiceDTO>(pagedInvoices, pageNumber, pageSize, totalCount);
        }

        public async Task<PaginationResult<InvoiceDTO>> ListInvoicesByDateAsync(DateTime dateTime, int pageNumber, int pageSize)
        {
            var invoices = await _invoiceRepository.ListInvoicesByDateAsync(dateTime);
            var totalCount = invoices.Count;
            var pagedInvoices = invoices
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PaginationResult<InvoiceDTO>(pagedInvoices, pageNumber, pageSize, totalCount);
        }

        public async Task VoidInvoiceAsync(int invoiceId)
        {
            await _invoiceRepository.VoidInvoiceAsync(invoiceId);
        }

        public async Task VoidOrderAsync(int orderId)
        {
            await _invoiceRepository.VoidOrderAsync(orderId);
        }

        public async Task<List<InvoiceOrdersDTO>> GetInvoiceOrdersAsync(int invoiceId)
        {
            return await _invoiceRepository.GetInvoiceOrdersAsync(invoiceId);
        }
    }
}
