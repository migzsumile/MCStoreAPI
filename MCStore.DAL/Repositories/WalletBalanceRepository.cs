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
    public class WalletBalanceRepository : IWalletBalanceRepository
    {
        private readonly MCStoreDbContext _context;

        public WalletBalanceRepository(MCStoreDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResult<WalletBalance>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.WalletBalance.AsNoTracking().OrderBy(w => w.Id);
            var total = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginationResult<WalletBalance>(items, pageNumber, pageSize, total);
        }

        public async Task<IEnumerable<WalletBalance>> GetAllLatestAsync()
        {
            var query = await _context.WalletBalance.AsNoTracking().OrderBy(w => w.WalletName).ToListAsync();
            return query;
        }

        public async Task<WalletBalance?> GetByIdAsync(int id)
        {
            return await _context.WalletBalance.FindAsync(id);
        }

        public async Task<WalletBalance> AddAsync(WalletBalance model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var walletName = model.WalletName?.Trim() ?? string.Empty;
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                // Use a transaction to ensure consistency when updating old records and fees
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Find existing wallets with same trimmed name (case-insensitive)
                    var existingWallets = await _context.WalletBalance
                        .Where(w => w.WalletName.Trim().ToLower() == walletName.ToLower() && w.IsCurrent == true)
                        .ToListAsync();

                    var oldIds = existingWallets.Select(w => w.Id).ToList();

                    if (existingWallets.Any())
                    {
                        // mark old wallets as not current
                        foreach (var w in existingWallets)
                        {
                            w.IsCurrent = false;
                        }
                        _context.WalletBalance.UpdateRange(existingWallets);
                        await _context.SaveChangesAsync();
                    }

                    // Normalize and ensure new wallet is current
                    model.WalletName = walletName;
                    model.IsCurrent = true;

                    await _context.WalletBalance.AddAsync(model);
                    await _context.SaveChangesAsync();

                    // If there were older wallet rows, update CashInCashOutFee rows that referenced them
                    if (oldIds.Any())
                    {
                        var feesToUpdate = await _context.CashInCashOutFee
                            .Where(f => f.WalletBalanceId.HasValue && oldIds.Contains(f.WalletBalanceId.Value))
                            .ToListAsync();

                        if (feesToUpdate.Any())
                        {
                            foreach (var fee in feesToUpdate)
                            {
                                fee.WalletBalanceId = model.Id;
                            }
                            _context.CashInCashOutFee.UpdateRange(feesToUpdate);
                            await _context.SaveChangesAsync();
                        }
                    }

                    await transaction.CommitAsync();
                    return model;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task UpdateAsync(int id, WalletBalance model)
        {
            var existing = await _context.WalletBalance.FindAsync(id);
            if (existing == null) throw new Exception("WalletBalance not found");

            existing.WalletName = model.WalletName;
            existing.Balance = model.Balance;
            existing.IsCurrent = model.IsCurrent;

            _context.WalletBalance.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.WalletBalance.FindAsync(id);
            if (existing == null) return false;

            _context.WalletBalance.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
