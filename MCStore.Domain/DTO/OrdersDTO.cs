using MCStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.DTO
{
    public class OrdersCreateDTO
    {
        public int StockId { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public string Comments { get; set; } = string.Empty;
    }

    public class OrdersListCreateDTO
    {
        public List<OrdersCreateDTO> Orders { get; set; } = new List<OrdersCreateDTO>();
    }

    public class OrderSearchResultDTO
    {
        public int StockId { get; set; }

        public int ProductId { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public decimal UnitPrice { get; set; }
    }

    public class OrderStocksAllocateDTO
    {
        public int StockId { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalPrice { get; set; } = 0;

        public bool IsMaxQuantity { get; set; }
    }

    public class OrdersProductIsDigitalDTO
    {
        public double BankFee { get; set; }

        public bool IsDigital { get; set; }
    }

    public class OrderListItemDTO
    {
        public int OrderId { get; set; }

        public int StockId { get; set; }

        public int InvoiceId { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Comments { get; set; }

        public string? OrderStatus { get; set; }
    }

    public class OrderListPagedDTO
    {
        public IEnumerable<OrderListItemDTO> Items { get; set; } = new List<OrderListItemDTO>();

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
