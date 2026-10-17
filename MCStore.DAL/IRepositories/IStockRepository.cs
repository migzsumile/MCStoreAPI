using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.IRepositories
{
    public interface IStockRepository
    {
        Task<List<StockDTO>> GetAllStocksAvailable();
        Task<List<StockDTO>> GetAllStocksExpired();
        Task AddStocksAsync(List<Stocks> s);
        Task UpdateStockAsync(int id, Stocks stock);
        Task<int> DeleteStock(int id);
        Task<List<StockDTO>> GetStocksByProductAsync(string searchString);

    }
}
