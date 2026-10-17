using MCStore.Business.IServices;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var a = await _productRepository.GetAllProductsAsync();
            return a;
        }

        public async Task<PaginationResult<Product>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;


            var products = await _productRepository.GetAllProductsAsync();
            var totalCount = products.Count();

            var result = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PaginationResult<Product>(result, pageNumber, pageSize, totalCount);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            return product ?? throw new Exception("No products found");
        }

        public async Task<Product> GetProductByProductCode(string productCode)
        {
            var product = await _productRepository.GetProductByProductCode(productCode);
            return product ?? new Product();
        }

        public async Task<PaginationResult<Product>> SearchProductByCodeName(int pageNumber, int pageSize, string searchString)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var products = await _productRepository.SearchProductByCodeName(searchString);
            var totalCount = products.Count();

            var result = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PaginationResult<Product>(result, pageNumber, pageSize, totalCount);
        }

        public async Task AddProductAsync(List<Product> products)
        {
            await _productRepository.AddProductAsync(products);
        }

        public async Task UpdateProductAsync(int id, Product p)
        {
            await _productRepository.UpdateProductAsync(id, p);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

    }
}
