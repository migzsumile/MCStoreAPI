using MCStore.Business.IServices;
using MCStore.DAL.IRepositories;
using MCStore.Domain.DTO;
using MCStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MCStore.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IInvoiceService invoiceService, IOrderRepository orderRepository)
        {
            _invoiceService = invoiceService;
            _orderRepository = orderRepository;
        }

        public async Task<OrderListPagedDTO> GetOrdersFilteredAsync(string? searchString, DateTime? transactionDate, int pageNumber, int pageSize)
        {
            return await _orderRepository.GetOrdersFilteredAsync(searchString, transactionDate, pageNumber, pageSize);
        }

        public async Task AddOrdersAsync(List<Orders> orders)
        {
            await _orderRepository.AddOrdersAsync(orders);
        }

        public async Task AddInvoiceAsync(List<Domain.Entities.Orders> orders, string? customerName, decimal totalAmount)
        {
            var finalOrders = new List<Orders>();

            var invoice = new Invoice
            {
                CustomerName = customerName ?? string.Empty,
                TransactionDate = DateTime.Now,
                Status = Domain.Enums.InvoiceStatus.Success,
                Orders = new List<Orders>()
            };

            // Preload referenced stocks via repository
            var referencedStockIds = orders.Select(x => x.StockId).Distinct().ToList();
            var referencedStocks = await _orderRepository.GetStocksByIdsAsync(referencedStockIds);

            var stockById = referencedStocks.ToDictionary(s => s.Id);

            // Validate totals per product (skip digital)
            var productIds = referencedStocks.Select(s => s.ProductId).Distinct().ToList();

            foreach (var pid in productIds)
            {
                var isDigital = await _orderRepository.IsProductDigitalAsync(pid);
                if (isDigital) continue;

                var totalRequestedForProduct = orders
                    .Where(o => stockById.ContainsKey(o.StockId) && stockById[o.StockId].ProductId == pid)
                    .Sum(o => o.Quantity);

                var totalAvailableForProduct = await _orderRepository.GetTotalAvailableForProductAsync(pid);

                if (totalRequestedForProduct > totalAvailableForProduct)
                {
                    var prodName = referencedStocks.FirstOrDefault(s => s.ProductId == pid)?.Product?.ProductName;
                    throw new Exception($"Insufficient total stock available for product {prodName}");
                }
            }

            var allocatedByStockId = new Dictionary<int, decimal>();

            foreach (var o in orders)
            {
                if (!stockById.TryGetValue(o.StockId, out var stock))
                    throw new Exception($"Stock not found for stockId {o.StockId}");

                var cashFee = await _orderRepository.GetCashInCashOutFeeByProductIdAsync(stock.ProductId);

                if (cashFee != null)
                {
                    if (!cashFee.WalletBalanceId.HasValue)
                        throw new Exception("WalletBalanceId is not set for digital product");

                    var walletBalance = await _orderRepository.GetWalletBalanceAsync(cashFee.WalletBalanceId.Value);
                    if (walletBalance == null)
                        throw new Exception("Wallet balance not found for digital product");

                    switch (cashFee.Operator)
                    {
                        case Domain.Enums.Operators.add:
                            walletBalance.Balance += o.Quantity;
                            break;
                        case Domain.Enums.Operators.subtract:
                            walletBalance.Balance -= o.Quantity;
                            break;
                        case Domain.Enums.Operators.multiply:
                            walletBalance.Balance *= o.Quantity;
                            break;
                        case Domain.Enums.Operators.divide:
                            walletBalance.Balance /= o.Quantity;
                            break;
                        default:
                            throw new Exception($"Unsupported operator {cashFee.Operator} for digital product {stock.Product?.ProductName}");
                    }

                    finalOrders.Add(new Orders
                    {
                        StockId = stock.Id,
                        UnitPrice = o.UnitPrice,
                        Quantity = o.Quantity,
                        TotalPrice = o.TotalPrice,
                        Comments = o.Comments,
                        TransactionDate = o.TransactionDate,
                        OrderStatus = o.OrderStatus,
                        Stock = stock,
                        Invoice = invoice
                    });
                    continue;
                }

                decimal requested = o.Quantity;

                allocatedByStockId.TryGetValue(stock.Id, out var alreadyAllocatedOnOriginal);
                var availableOnOriginal = stock.UnitInStock;

                if (availableOnOriginal >= requested)
                {
                    allocatedByStockId[stock.Id] = alreadyAllocatedOnOriginal + requested;
                    stock.UnitInStock -= requested;

                    finalOrders.Add(new Orders
                    {
                        StockId = stock.Id,
                        UnitPrice = o.UnitPrice,
                        Quantity = requested,
                        TotalPrice = o.TotalPrice,
                        Comments = o.Comments,
                        TransactionDate = o.TransactionDate,
                        OrderStatus = o.OrderStatus,
                        Stock = stock,
                        Invoice = invoice
                    });
                    continue;
                }

                var replacement = await _orderRepository.GetAvailableStocksForProductAsync(stock.ProductId);

                Stocks? chosen = null;
                foreach (var candidate in replacement)
                {
                    allocatedByStockId.TryGetValue(candidate.Id, out var alreadyAllocated);
                    var available = candidate.UnitInStock;
                    if (available >= requested)
                    {
                        chosen = candidate;
                        break;
                    }
                }

                if (chosen == null)
                {
                    throw new Exception($"Insufficient single-batch stock available for product {stock.Product?.ProductName}");
                }

                allocatedByStockId.TryGetValue(chosen.Id, out var prevAlloc);
                allocatedByStockId[chosen.Id] = prevAlloc + requested;
                chosen.UnitInStock -= requested;

                finalOrders.Add(new Orders
                {
                    StockId = chosen.Id,
                    UnitPrice = o.UnitPrice,
                    Quantity = requested,
                    TotalPrice = o.TotalPrice,
                    Comments = o.Comments,
                    TransactionDate = o.TransactionDate,
                    OrderStatus = o.OrderStatus
                    ,
                    Stock = chosen,
                    Invoice = invoice
                });
            }

            // attach collected orders to invoice and set total
            invoice.Orders = finalOrders;
            invoice.TotalAmount = finalOrders.Sum(x => x.TotalPrice);

            // Persist via invoice service (which will call repository and SaveChanges)
            await _invoice_service_create(invoice);
        }

        // helper shim to call invoice service in a more testable way
        private async Task _invoice_service_create(Invoice invoice)
        {
            await _invoiceService.CreateInvoice(invoice);
        }

        public async Task<OrderSearchResultDTO> GetStockOrderByCodeNameAsync(string searchString)
        {
            return await _orderRepository.GetStockOrderByCodeNameAsync(searchString);
        } 
        
        public async Task<OrdersProductIsDigitalDTO> BankFeeAsync(string productCode, double amount)
        {
            return await _orderRepository.BankFeeAsync(productCode, amount);
        }

        public async Task<OrderStocksAllocateDTO> GetTotalPriceByQuantity(string productCode, double quantity)
        {
            return await _orderRepository.GetTotalPriceByQuantity(productCode, quantity);
        }

    }
}
