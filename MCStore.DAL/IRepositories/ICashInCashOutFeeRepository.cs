using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.IRepositories
{
    public interface ICashInCashOutFeeRepository
    {
        Task<PaginationResult<CashInCashOutFee>> GetAllAsync(int pageNumber = 1, int pageSize = 10);

        Task<CashInCashOutFee> GetByIdAsync(int id);

        Task<IEnumerable<WalletBalance>> GetAllLatestWalletBalancesAsync();

        Task<PaginationResult<CashInCashOutFee>> GetByProductIdAsync(int productId, int pageNumber = 1, int pageSize = 10);
        Task<PaginationResult<CashInCashOutFee>> SearchAsync(string searchString, int pageNumber = 1, int pageSize = 10);

        Task AddAsync(List<CashInCashOutFee> items);

        Task UpdateAsync(int id, CashInCashOutFee item);

        Task<bool> DeleteAsync(int id);
    }
}
