using MCStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.Entities
{
    [Table("Orders", Schema = "Store")]
    public class Orders
    {
        public int Id { get; set; }

        public int StockId { get; set; }

        public int InvoiceId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public string Comments { get; set; } = "";

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public InvoiceStatus OrderStatus { get; set; } = InvoiceStatus.Success;

        public Stocks? Stock { get; set; }

        public Invoice? Invoice { get; set; }
    }

    public class OrdersList
    {
        public required List<Orders> Orders { get; set; }
    }

}
