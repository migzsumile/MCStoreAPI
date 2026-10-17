using MCStore.Business.IServices;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.Services
{
    public class WalletBalanceService : IWalletBalanceService
    {
        private readonly IWalletBalanceRepository _walletBalanceRepository;

        public WalletBalanceService(IWalletBalanceRepository walletBalanceRepository)
        {
            _walletBalanceRepository = walletBalanceRepository;
        }

        public async Task<PaginationResult<WalletBalance>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _walletBalanceRepository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<WalletBalance>> GetAllLatestAsync()
        {
            return await _walletBalanceRepository.GetAllLatestAsync();
        }

        public async Task<WalletBalance?> GetByIdAsync(int id)
        {
            return await _walletBalanceRepository.GetByIdAsync(id);
        }

        public async Task<WalletBalance> AddAsync(WalletBalance model)
        {
            return await _walletBalanceRepository.AddAsync(model);
        }

        public async Task UpdateAsync(int id, WalletBalance model)
        {
            await _walletBalanceRepository.UpdateAsync(id, model);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _walletBalanceRepository.DeleteAsync(id);
        }
    }
}
