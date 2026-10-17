using MCStore.DAL.Context;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using MCStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly MCStoreDbContext _context;

        public InvoiceRepository(MCStoreDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateInvoice(Invoice invoice)
        {
            await _context.Invoice.AddAsync(invoice);
            _context.SaveChanges();

            return invoice.Id;
        }

        public async Task<List<InvoiceDTO>> ListInvoicesByAsync()
        {
            var invoices = await _context.Invoice
                .Include(i => i.Orders)
                .OrderByDescending(x => x.TransactionDate)
                .AsNoTracking()
                .Select(i => new InvoiceDTO 
                {
                    Id = i.Id,
                    CustomerName = i.CustomerName,
                    TotalAmount = i.TotalAmount,
                    TransactionDate = i.TransactionDate,
                    Status = i.Status.ToString(),
                    Orders = i.Orders
                }).ToListAsync();

            return invoices;
        }

        public async Task<List<InvoiceDTO>> ListInvoicesByDateAsync(DateTime dateTime)
        {
            var invoices = await _context.Invoice
                .Include(i => i.Orders)
                .Where(i => i.TransactionDate.Date == dateTime.Date)
                .OrderByDescending(x => x.TransactionDate)
                .Select(i => new InvoiceDTO
                {
                    Id = i.Id,
                    CustomerName = i.CustomerName,
                    TotalAmount = i.TotalAmount,
                    TransactionDate = i.TransactionDate,
                    Status = i.Status.ToString(),
                    Orders = i.Orders
                }).ToListAsync();

            return invoices;
        }

        public async Task VoidInvoiceAsync(int invoiceId)
        {
            var invoice = await _context.Invoice
                .Include(i => i.Orders)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (invoice == null)
            {
                throw new Exception("Invoice not found.");
            }
            // Update the status of the invoice to void
            invoice.Status = InvoiceStatus.Void;
            // Update the stock quantities for each order in the invoice
            foreach (var order in invoice.Orders)
            {
                order.OrderStatus = InvoiceStatus.Void;
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == order.StockId);
                if (stock != null)
                {
                    stock.UnitInStock += order.Quantity;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task VoidOrderAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }
            order.OrderStatus = InvoiceStatus.Void;
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == order.StockId);
            if (stock != null)
            {
                stock.UnitInStock += order.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<InvoiceOrdersDTO>> GetInvoiceOrdersAsync(int invoiceId)
        {
            var orders = await _context.Orders
                .Where(o => o.InvoiceId == invoiceId)
                .Include(o => o.Stock.Product)
                .Select(o => new InvoiceOrdersDTO
                {
                    OrderId = o.Id,
                    StockId = o.StockId,
                    ProductCode = o.Stock.Product.ProductCode,
                    ProductName = o.Stock.Product.ProductName,
                    UnitPrice = o.UnitPrice,
                    Quantity = o.Quantity,
                    TotalPrice = o.TotalPrice,
                    Status = o.OrderStatus.ToString()
                })
                .ToListAsync();

            return orders;
        }
    }
}
