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
    public class CashInCashOutFeeRepository : ICashInCashOutFeeRepository
    {
        private readonly MCStoreDbContext _context;

        public CashInCashOutFeeRepository(MCStoreDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResult<CashInCashOutFee>> SearchAsync(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            searchString = (searchString ?? string.Empty).Trim();
            var query = _context.CashInCashOutFee.Include(x => x.Product).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (x.Product != null && (
                    (!string.IsNullOrEmpty(x.Product.ProductName) && x.Product.ProductName.Contains(searchString)) ||
                    (!string.IsNullOrEmpty(x.Product.ProductCode) && x.Product.ProductCode.Contains(searchString))
                )));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(x => x.StartAmount)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<CashInCashOutFee>(items, pageNumber, pageSize, totalCount);
        }

        public async Task<PaginationResult<CashInCashOutFee>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.CashInCashOutFee.Include(x => x.Product).Include(x => x.WalletBalance).AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<CashInCashOutFee>(items, pageNumber, pageSize, totalCount);
        }

        public async Task<IEnumerable<WalletBalance>> GetAllLatestWalletBalancesAsync()
        {
            var query = await _context.WalletBalance.AsNoTracking().OrderBy(w => w.WalletName).ToListAsync();
            return query;
        }

        public async Task<CashInCashOutFee> GetByIdAsync(int id)
        {
            var item = await _context.CashInCashOutFee.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) throw new Exception("CashInCashOutFee not found");
            return item;
        }

        public async Task<PaginationResult<CashInCashOutFee>> GetByProductIdAsync(int productId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.CashInCashOutFee.Where(x => x.ProductId == productId).Include(x => x.Product).AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(x => x.StartAmount)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<CashInCashOutFee>(items, pageNumber, pageSize, totalCount);
        }

        public async Task AddAsync(List<CashInCashOutFee> items)
        {
            if (items == null || !items.Any()) return;

            foreach (var item in items)
            {
                if (item.StartAmount > item.EndAmount)
                    throw new ArgumentException($"StartAmount must be <= EndAmount for productId {item.ProductId}");
                if (item.Fee < 0)
                    throw new ArgumentException($"Fee must be >= 0 for productId {item.ProductId}");

                var productExists = await _context.Product.AnyAsync(p => p.Id == item.ProductId);
                if (!productExists)
                    throw new ArgumentException($"Product with id {item.ProductId} does not exist");

                // check overlap with existing entries
                var overlap = await _context.CashInCashOutFee
                    .AnyAsync(x => x.ProductId == item.ProductId && !(item.EndAmount < x.StartAmount || item.StartAmount > x.EndAmount));
                if (overlap)
                    throw new ArgumentException($"Fee range overlaps with existing range for productId {item.ProductId}");
            }

            // check overlap within batch
            var grouped = items.GroupBy(x => x.ProductId);
            foreach (var g in grouped)
            {
                var list = g.OrderBy(x => x.StartAmount).ToList();
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].StartAmount <= list[i - 1].EndAmount)
                        throw new ArgumentException($"Fee ranges in the provided batch overlap for productId {g.Key}");
                }
            }

            await _context.CashInCashOutFee.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, CashInCashOutFee item)
        {
            try
            {
                var existing = await _context.CashInCashOutFee.FindAsync(id);
                if (existing == null) throw new Exception("CashInCashOutFee not found");

                if (item.StartAmount > item.EndAmount)
                    throw new ArgumentException("StartAmount must be <= EndAmount");
                if (item.Fee < 0)
                    throw new ArgumentException("Fee must be >= 0");

                var productExists = await _context.Product.AnyAsync(p => p.Id == item.ProductId);
                if (!productExists)
                    throw new ArgumentException($"Product with id {item.ProductId} does not exist");

                // check overlap with other existing entries for same product excluding this id
                var overlap = await _context.CashInCashOutFee
                    .AnyAsync(x => x.ProductId == item.ProductId && x.Id != id && !(item.EndAmount < x.StartAmount || item.StartAmount > x.EndAmount));
                if (overlap)
                    throw new ArgumentException($"Fee range overlaps with existing range for productId {item.ProductId}");

                existing.ProductId = item.ProductId;
                existing.StartAmount = item.StartAmount;
                existing.EndAmount = item.EndAmount;
                existing.Fee = item.Fee;
                existing.WalletBalanceId = item.WalletBalanceId;
                existing.Operator = item.Operator;

                _context.CashInCashOutFee.Update(existing);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.CashInCashOutFee.FindAsync(id);
            if (existing == null) return false;
            _context.CashInCashOutFee.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
