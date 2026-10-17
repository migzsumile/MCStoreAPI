using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.IServices
{
    public interface IStockService
    {
        Task<PaginationResult<StockDTO>> GetAllStocksAvailable(int pageNumber, int pageSize);
        Task<PaginationResult<StockDTO>> GetAllStocksExpired(int pageNumber, int pageSize);
        Task AddStocksAsync(List<Stocks> s);
        Task UpdateStockAsync(int id, Stocks stock);
        Task<int> DeleteStock(int id);
        Task<PaginationResult<StockDTO>> GetStocksByProductAsync(int pageNumber, int pageSize, string searchString);
    }
}
