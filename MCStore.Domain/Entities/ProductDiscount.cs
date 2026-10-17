using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.Entities
{
    [Table("ProductDiscount", Schema = "Store")]
    public class ProductDiscount
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public double Quantity { get; set; }

        public double DiscountedPrice { get; set; } = 0;

        public bool IsCurrent { get; set; }

    }
}
