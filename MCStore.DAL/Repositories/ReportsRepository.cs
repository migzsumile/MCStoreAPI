using MCStore.DAL.Context;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly MCStoreDbContext _context;

        public ReportsRepository(MCStoreDbContext context)
        {
            _context = context;
        }

        public async Task<SalesReportDTO> GetSalesReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate?.Date ?? DateTime.MinValue.Date;
            var end = endDate?.Date ?? DateTime.MaxValue.Date;

            var digitalProductIds = await _context.CashInCashOutFee
                .Select(x => x.ProductId)
                .Distinct()
                .ToListAsync();

            var salesInRange = _context.Orders
                .Include(s => s.Stock)
                .Where(s => s.TransactionDate.Date >= start && s.TransactionDate.Date <= end);

            var normalSalesQuery = salesInRange.Where(s => !digitalProductIds.Contains(s.Stock.ProductId));
            var digitalSalesQuery = salesInRange.Where(s => digitalProductIds.Contains(s.Stock.ProductId));

            var netSalesNormal = await normalSalesQuery.SumAsync(s => (decimal?)s.TotalPrice) ?? 0m;
            var costOfNormal = await normalSalesQuery.SumAsync(s => (decimal?)(s.Quantity * (s.Stock.Product != null ? s.Stock.Product.CapitalPrice : 0m))) ?? 0m;
            var margin = netSalesNormal - costOfNormal;
            var netSalesDigital = await digitalSalesQuery.SumAsync(s => ((decimal?)s.TotalPrice - s.Quantity)) ?? 0m;

            var profit = margin + netSalesDigital;

            return new SalesReportDTO
            {
                StartDate = start,
                EndDate = end,
                NetSalesNormal = netSalesNormal,
                CostOfNormalProductSold = costOfNormal,
                Margin = margin,
                NetSalesDigital = netSalesDigital,
                Profit = profit
            };
        }

        public async Task<StockInsightsDTO> GetStockInsightsAsync(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate?.Date ?? DateTime.MinValue.Date;
            var end = endDate?.Date ?? DateTime.MaxValue.Date;

            var digitalProductIds = await _context.CashInCashOutFee
                .Select(x => x.ProductId)
                .Distinct()
                .ToListAsync();

            // Total inventory price = sum of UnitInStock * UnitPrice for non-expired stocks
            var totalInventoryPrice = await _context.Stocks
                .Where(s => s.UnitInStock > 0 && s.ExpiryDate > DateTime.Now && s.IsExpired == false && !digitalProductIds.Contains(s.ProductId))
                .SumAsync(s => (decimal?)(s.UnitInStock * s.UnitPrice)) ?? 0m;

            // Inventory cost = sum of UnitInStock * CapitalPrice
            var inventoryCost = await _context.Stocks
                .Where(s => s.UnitInStock > 0 && s.ExpiryDate > DateTime.Now && s.IsExpired == false && !digitalProductIds.Contains(s.ProductId))
                .SumAsync(s => (decimal?)(s.UnitInStock * s.CapitalPrice)) ?? 0m;

            var potentialMargin = totalInventoryPrice - inventoryCost;

            // Top 10 moving products by quantity sold in the date range
            var topMoving = await _context.Orders
                .Include(s => s.Stock)
                .Where(s => s.TransactionDate.Date >= start && s.TransactionDate.Date <= end)
                .GroupBy(s => s.Stock.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .OrderByDescending(g => g.Quantity)
                .Take(10)
                .Join(_context.Product,
                      g => g.ProductId,
                      p => p.Id,
                      (g, p) => new TopMovingProductDTO
                      {
                          Product = p.ProductName,
                          Quantity = g.Quantity,
                          UnitPrice = p.UnitPrice,
                          CapitalPrice = p.CapitalPrice,
                          Margin = (p.UnitPrice - p.CapitalPrice) * g.Quantity
                      })
                .ToListAsync();

            return new StockInsightsDTO
            {
                StartDate = start,
                EndDate = end,
                TotalInventoryPrice = totalInventoryPrice,
                InventoryCost = inventoryCost,
                PotentialSalesMargin = potentialMargin,
                TopMovingProducts = topMoving
            };
        }
    }
}
