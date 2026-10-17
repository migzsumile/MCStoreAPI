using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public string Status { get; set; }

        public ICollection<Orders> Orders { get; set; }
    }

    public class InvoiceOrdersDTO
    {
        public int OrderId { get; set; }

        public int StockId { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalPrice { get; set; } = 0;

        public string Status { get; set; }
    }
}
