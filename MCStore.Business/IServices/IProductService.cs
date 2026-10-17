using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.IServices
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<PaginationResult<Product>> GetAllProductsAsync(int pageNumber, int pageSize);

        Task<Product> GetProductByIdAsync(int id);

        Task<Product> GetProductByProductCode(string productCode);

        Task<PaginationResult<Product>> SearchProductByCodeName(int pageNumber, int pageSize, string searchString);

        Task AddProductAsync(List<Product> products);

        Task UpdateProductAsync(int id, Product p);
        Task<bool> DeleteProductAsync(int id);
    }
}
