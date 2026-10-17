using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.IServices
{
    public interface IOrderService
    {
        Task AddOrdersAsync(List<Orders> orders);

        Task AddInvoiceAsync(List<Orders> orders, string? customerName, decimal totalAmount);

        Task<OrderSearchResultDTO> GetStockOrderByCodeNameAsync(string searchString);

        Task<OrdersProductIsDigitalDTO> BankFeeAsync(string productCode, double amount);

        Task<OrderStocksAllocateDTO> GetTotalPriceByQuantity(string productCode, double quantity);

        Task<OrderListPagedDTO> GetOrdersFilteredAsync(string? searchString, DateTime? transactionDate, int pageNumber, int pageSize);
    }
}
