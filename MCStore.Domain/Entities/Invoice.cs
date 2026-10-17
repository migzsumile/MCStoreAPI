using MCStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.Entities
{
    [Table("Invoice", Schema = "Store")]
    public class Invoice
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public int Payment_TypeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public InvoiceStatus Status { get; set; }

        public ICollection<Orders> Orders { get; set; }

    }
}
