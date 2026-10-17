using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DomainClasses.Entities
{
    [Table("Product", Schema = "Store")]
    public class Product
    {
        public int Id { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        //public int CategoryId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CapitalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitInStock { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
