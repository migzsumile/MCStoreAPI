using MCStore.Business.IServices;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.Services
{
    public class CashInCashOutFeeService : ICashInCashOutFeeService
    {
        private readonly ICashInCashOutFeeRepository _repository;

        public CashInCashOutFeeService(ICashInCashOutFeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResult<CashInCashOutFee>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<CashInCashOutFee> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<WalletBalance>> GetAllLatestWalletBalancesAsync()
        {
            return await _repository.GetAllLatestWalletBalancesAsync();
        }

        public async Task<PaginationResult<CashInCashOutFee>> GetByProductIdAsync(int productId, int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetByProductIdAsync(productId, pageNumber, pageSize);
        }

        public async Task<PaginationResult<CashInCashOutFee>> SearchAsync(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.SearchAsync(searchString, pageNumber, pageSize);
        }

        public async Task AddAsync(List<CashInCashOutFee> items)
        {
            await _repository.AddAsync(items);
        }

        public async Task UpdateAsync(int id, CashInCashOutFee item)
        {
            await _repository.UpdateAsync(id, item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
