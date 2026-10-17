using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.Entities
{
    [Table("Stocks", Schema = "Store")]
    public class Stocks
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitInStock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CapitalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsExpired { get; set; }

        public required Product Product { get; set; }

    }
}
