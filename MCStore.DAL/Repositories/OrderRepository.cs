using MCStore.DAL.Context;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MCStoreDbContext _context;

        public OrderRepository(MCStoreDbContext context)
        {
            _context = context;
        }

        public async Task<OrderListPagedDTO> GetOrdersFilteredAsync(string? searchString, DateTime? transactionDate, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Orders
                .Include(o => o.Stock)
                    .ThenInclude(s => s.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var pattern = $"%{searchString.Trim()}%";
                query = query.Where(o =>
                    (o.Stock != null && o.Stock.Product != null &&
                        (EF.Functions.Like(o.Stock.Product.ProductCode, pattern) ||
                         EF.Functions.Like(o.Stock.Product.ProductName, pattern)))
                    || EF.Functions.Like(o.Comments ?? string.Empty, pattern)
                );
            }

            if (transactionDate.HasValue)
            {
                var start = transactionDate.Value.Date;
                var end = start.AddDays(1);
                query = query.Where(o => o.TransactionDate >= start && o.TransactionDate < end);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.TransactionDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderListItemDTO
                {
                    OrderId = o.Id,
                    StockId = o.StockId,
                    InvoiceId = o.InvoiceId,
                    ProductCode = o.Stock != null && o.Stock.Product != null ? o.Stock.Product.ProductCode : null,
                    ProductName = o.Stock != null && o.Stock.Product != null ? o.Stock.Product.ProductName : null,
                    UnitPrice = o.UnitPrice,
                    Quantity = o.Quantity,
                    TotalPrice = o.TotalPrice,
                    TransactionDate = o.TransactionDate,
                    Comments = o.Comments,
                    OrderStatus = o.OrderStatus.ToString()
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new OrderListPagedDTO
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task AddOrdersAsync(List<Orders> orders)
        {
            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();
        }

        public async Task AddInvoiceAsync(List<Orders> orders, string? customerName, decimal totalAmount)
        {
            var invoice = new Invoice
            {
                CustomerName = customerName ?? string.Empty,
                TotalAmount = totalAmount,
                TransactionDate = DateTime.Now,
                Status = Domain.Enums.InvoiceStatus.Success,
                Orders = orders
            };

            await _context.Invoice.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderSearchResultDTO> GetStockOrderByCodeNameAsync(string searchString)
        {
            var pattern = $"%{searchString}%";
            var stock = await _context.Stocks
                .Include(s => s.Product)
                .Where(s => EF.Functions.Like(s.Product.ProductCode, pattern) || EF.Functions.Like(s.Product.ProductName, pattern))
                .Select(s => new OrderSearchResultDTO
                {
                    StockId = s.Id,
                    ProductId = s.ProductId,
                    ProductCode = s.Product.ProductCode,
                    ProductName = s.Product.ProductName,
                    UnitPrice = s.UnitPrice
                })
                .FirstOrDefaultAsync();

            return stock ?? new OrderSearchResultDTO();
        }

        public async Task<OrdersProductIsDigitalDTO> BankFeeAsync(string productCode, double amount)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductCode == productCode);
            if (product == null) return new OrdersProductIsDigitalDTO { IsDigital = false, BankFee = 0 };

            var fee = await _context.CashInCashOutFee.FirstOrDefaultAsync(c => c.ProductId == product.Id && (double)c.StartAmount <= amount && (double)c.EndAmount >= amount);
            if (fee == null) return new OrdersProductIsDigitalDTO { IsDigital = false, BankFee = 0 };

            return new OrdersProductIsDigitalDTO { IsDigital = true, BankFee = (double)fee.Fee };
        }

        public async Task<OrderStocksAllocateDTO> GetTotalPriceByQuantity(string productCode, double quantity)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductCode == productCode);
            if (product == null) return new OrderStocksAllocateDTO();

            var stock = await _context.Stocks.Where(s => s.ProductId == product.Id && s.ExpiryDate > DateTime.Now && s.IsExpired == false && s.UnitInStock >= (decimal)quantity)
                .OrderBy(s => s.ExpiryDate)
                .FirstOrDefaultAsync();

            if (stock == null) return new OrderStocksAllocateDTO();

            var totalPrice = (double)stock.UnitPrice * quantity;

            var productDiscount = await GetDiscountPriceByQuantity(stock.ProductId, quantity);
            if (productDiscount != null && productDiscount.DiscountedPrice > 0)
            {
                var bundles = (int)quantity / (int)productDiscount.Quantity;
                var remainder = quantity % productDiscount.Quantity;
                totalPrice = (bundles * productDiscount.DiscountedPrice) + (remainder * (double)stock.UnitPrice);
            }

            return new OrderStocksAllocateDTO
            {
                StockId = stock.Id,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                UnitPrice = stock.UnitPrice,
                Quantity = (decimal)quantity,
                TotalPrice = (decimal)totalPrice,
                IsMaxQuantity = true
            };
        }

        public async Task<List<Stocks>> GetStocksByIdsAsync(IEnumerable<int> stockIds)
        {
            return await _context.Stocks
                .Include(s => s.Product)
                .Where(s => stockIds.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<bool> IsProductDigitalAsync(int productId)
        {
            return await _context.CashInCashOutFee.AnyAsync(x => x.ProductId == productId);
        }

        public async Task<decimal> GetTotalAvailableForProductAsync(int productId)
        {
            return await _context.Stocks
                .Where(s => s.ProductId == productId && s.ExpiryDate > DateTime.Now && s.IsExpired == false && s.UnitInStock > 0)
                .SumAsync(s => (decimal?)s.UnitInStock) ?? 0m;
        }

        public async Task<List<Stocks>> GetAvailableStocksForProductAsync(int productId)
        {
            return await _context.Stocks
                .Where(s => s.ProductId == productId && s.ExpiryDate > DateTime.Now && s.IsExpired == false && s.UnitInStock > 0)
                .OrderBy(s => s.ExpiryDate)
                .ToListAsync();
        }

        public async Task<WalletBalance?> GetWalletBalanceAsync(int walletBalanceId)
        {
            return await _context.WalletBalance.FirstOrDefaultAsync(x => x.Id == walletBalanceId && x.IsCurrent == true);
        }

        public async Task<CashInCashOutFee?> GetCashInCashOutFeeByProductIdAsync(int productId)
        {
            return await _context.CashInCashOutFee.FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        private async Task<ProductDiscount> GetDiscountPriceByQuantity(int productId, double quantity)
        {
            var discount = await _context.ProductDiscount
                .Where(d => d.ProductId == productId && d.Quantity <= quantity && d.IsCurrent)
                .SingleOrDefaultAsync();

            return discount;
        }


    }
}
