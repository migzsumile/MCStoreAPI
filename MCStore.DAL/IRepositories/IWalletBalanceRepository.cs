using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.IRepositories
{
    public interface IWalletBalanceRepository
    {
        Task<PaginationResult<WalletBalance>> GetAllAsync(int pageNumber, int pageSize);

        Task<IEnumerable<WalletBalance>> GetAllLatestAsync();

        Task<WalletBalance?> GetByIdAsync(int id);

        Task<WalletBalance> AddAsync(WalletBalance model);

        Task UpdateAsync(int id, WalletBalance model);

        Task<bool> DeleteAsync(int id);
    }
}
