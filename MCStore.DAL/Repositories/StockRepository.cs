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
    public class StockRepository : IStockRepository
    {
        private readonly MCStoreDbContext _context;
        public StockRepository(MCStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockDTO>> GetAllStocksAvailable()
        {
            var dateToday = DateTime.Today;
            var stocks = await _context.Stocks
                .Where(x => x.Product != null && x.Product.ProductCode != null && x.ExpiryDate >= dateToday && x.UnitInStock > 0)
                .Select(x => new StockDTO
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductCode = x.Product.ProductCode,
                    ProductName = x.Product.ProductName,
                    InitialQuantity = x.InitialQuantity,
                    CapitalPrice = x.CapitalPrice,
                    UnitInStock = x.UnitInStock,
                    UnitPrice = x.UnitPrice,
                    ExpiryDate = x.ExpiryDate
                }).ToListAsync();

            return stocks;
        }

        public async Task<List<StockDTO>> GetAllStocksExpired()
        {
            var dateToday = DateTime.Today;
            var stocks = await _context.Stocks
                .Where(x => x.Product != null && x.Product.ProductCode != null && x.ExpiryDate < dateToday && x.UnitInStock > 0)
                .Select(x => new StockDTO
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductCode = x.Product.ProductCode,
                    ProductName = x.Product.ProductName,
                    InitialQuantity = x.InitialQuantity,
                    CapitalPrice = x.CapitalPrice,
                    UnitInStock = x.UnitInStock,
                    UnitPrice = x.UnitPrice,
                    ExpiryDate = x.ExpiryDate
                }).ToListAsync();

            return stocks;
        }

        public async Task AddStocksAsync(List<Stocks> s)
        {
            await _context.Stocks.AddRangeAsync(s);
            _context.SaveChanges();
        }

        public async Task UpdateStockAsync(int id, Stocks updateStock)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                throw new InvalidOperationException($"Stock with ID {id} not found.");
            }

            stock.InitialQuantity = updateStock.InitialQuantity;
            stock.UnitInStock = updateStock.UnitInStock;
            stock.CapitalPrice = updateStock.CapitalPrice;
            stock.UnitPrice = updateStock.UnitPrice;
            stock.ExpiryDate = updateStock.ExpiryDate;
            stock.IsExpired = updateStock.IsExpired;

            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteStock(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                throw new InvalidOperationException($"Stock with ID {id} not found.");
            }

            _context.Stocks.Remove(stock);
            return _context.SaveChanges();
        }

        public async Task<Stocks> GetStockByIdAsync(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                throw new InvalidOperationException($"Stock with ID {id} not found.");
            }
            return stock;
        }

        public async Task<List<StockDTO>> GetStocksByProductAsync(string searchString)
        {
            var dateToday = DateTime.Today;
            var stocks = await _context.Stocks.Where(x => x.Product != null && x.Product.ProductCode != null
                && (EF.Functions.Like(x.Product.ProductCode, $"%{searchString}%") || EF.Functions.Like(x.Product.ProductName, $"%{searchString}%"))
                && x.ExpiryDate >= dateToday && x.UnitInStock > 0)
                .Select(x => new StockDTO
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductCode = x.Product.ProductCode,
                    ProductName = x.Product.ProductName,
                    InitialQuantity = x.InitialQuantity,
                    CapitalPrice = x.CapitalPrice,
                    UnitInStock = x.UnitInStock,
                    UnitPrice = x.UnitPrice,
                    ExpiryDate = x.ExpiryDate
                })
                .ToListAsync();

            return stocks;
        }

    }
}
