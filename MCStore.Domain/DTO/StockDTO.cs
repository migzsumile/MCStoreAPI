using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.DTO
{
    public class StockDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CapitalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitInStock { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }

    public class StocksList
    {
        public List<Stocks> Stocks { get; set; } = new List<Stocks>();
    }
}
