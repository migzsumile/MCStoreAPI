using MCStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.DTO
{
    public class ProductDTO
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
        public string CreatedBy { get; set; } = "Admin";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; } = "Admin";
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }

    public class ProductListDTO
    {
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
