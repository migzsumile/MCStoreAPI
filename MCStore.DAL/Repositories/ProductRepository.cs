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
    public class ProductRepository : IProductRepository
    {
        private readonly MCStoreDbContext _context;

        public ProductRepository(MCStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var a = await _context.Product.OrderBy(p => p.ProductName).ToListAsync();
            return a;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _context.Product.Where(x => x.Id == id).SingleOrDefaultAsync();

            return product ?? throw new Exception("No products found");
        }

        public async Task<Product> GetProductByProductCode(string productCode)
        {
            try
            {
                var product = await _context.Product.SingleOrDefaultAsync(x => x.ProductCode.Equals(productCode));
                return product ?? new Product();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<Product>> SearchProductByCodeName(string searchString)
        {
            try
            {
                var products = await _context.Product
                    .Where(x => x.ProductName.Contains(searchString) || x.ProductCode.Contains(searchString))
                    .OrderBy(x => x.ProductName)
                    .ToListAsync();

                return products;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task AddProductAsync(List<Product> products)
        {
            products?.ForEach(p =>
            {
                p.ProductCode = p.ProductCode.ToUpper();
                p.ProductName = p.ProductName.ToUpper();
            });

            await _context.Product.AddRangeAsync(products);

            var stocks = products.Select(p => new Stocks
            {
                Product = p,
                InitialQuantity = p.UnitInStock,
                UnitInStock = p.UnitInStock,
                CapitalPrice = p.CapitalPrice,
                UnitPrice = p.UnitPrice,
                ExpiryDate = p.ExpiryDate ?? DateTime.MaxValue,
                IsExpired = false
            }).ToList();

            await _context.Stocks.AddRangeAsync(stocks);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(int id, Product p)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.ProductCode = p.ProductCode;
            product.ProductName = p.ProductName;
            product.UnitPrice = p.UnitPrice;
            product.CapitalPrice = p.CapitalPrice;
            product.UnitInStock = p.UnitInStock;
            product.ExpiryDate = p.ExpiryDate;

            _context.Product.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
