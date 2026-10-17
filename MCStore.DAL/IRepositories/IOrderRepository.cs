using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.IRepositories
{
    public interface IOrderRepository
    {
        Task AddOrdersAsync(List<Orders> orders);

        Task AddInvoiceAsync(List<Orders> orders, string? customerName, decimal totalAmount);

        Task<List<Stocks>> GetStocksByIdsAsync(IEnumerable<int> stockIds);

        Task<bool> IsProductDigitalAsync(int productId);

        Task<decimal> GetTotalAvailableForProductAsync(int productId);

        Task<List<Stocks>> GetAvailableStocksForProductAsync(int productId);

        Task<WalletBalance?> GetWalletBalanceAsync(int walletBalanceId);

        Task<CashInCashOutFee?> GetCashInCashOutFeeByProductIdAsync(int productId);

        Task<OrderSearchResultDTO> GetStockOrderByCodeNameAsync(string searchString);

        Task<OrdersProductIsDigitalDTO> BankFeeAsync(string productCode, double amount);

        Task<OrderStocksAllocateDTO> GetTotalPriceByQuantity(string productCode, double quantity);

        Task<OrderListPagedDTO> GetOrdersFilteredAsync(string? searchString, DateTime? transactionDate, int pageNumber, int pageSize);
    }
}
