using MCStore.Business.IServices;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<PaginationResult<StockDTO>> GetAllStocksAvailable(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            var stocks = await _stockRepository.GetAllStocksAvailable();
            var totalCount = stocks.Count();
            var result = stocks
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginationResult<StockDTO>(result, pageNumber, pageSize, totalCount);
        }

        public async Task<PaginationResult<StockDTO>> GetAllStocksExpired(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            var stocks = await _stockRepository.GetAllStocksExpired();
            var totalCount = stocks.Count();
            var result = stocks
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginationResult<StockDTO>(result, pageNumber, pageSize, totalCount);
        }

        public async Task AddStocksAsync(List<Stocks> s)
        {
            await _stockRepository.AddStocksAsync(s);
        }

        public async Task UpdateStockAsync(int id, Stocks stock)
        {
            await _stockRepository.UpdateStockAsync(id, stock);
        }
        public async Task<int> DeleteStock(int id)
        {
            return await _stockRepository.DeleteStock(id);
        }

        public async Task<PaginationResult<StockDTO>> GetStocksByProductAsync(int pageNumber, int pageSize, string searchString)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            var stocks = await _stockRepository.GetStocksByProductAsync(searchString);
            var totalCount = stocks.Count();
            var result = stocks
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginationResult<StockDTO>(result, pageNumber, pageSize, totalCount);
        }
    }
}
