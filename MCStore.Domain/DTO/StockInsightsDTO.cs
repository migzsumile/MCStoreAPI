using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Domain.DTO
{
    public class TopMovingProductDTO
    {
        public string? Product { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal CapitalPrice { get; set; }

        public decimal Margin { get; set; }
    }

    public class StockInsightsDTO
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TotalInventoryPrice { get; set; }

        public decimal InventoryCost { get; set; }

        public decimal PotentialSalesMargin { get; set; }

        public IEnumerable<TopMovingProductDTO> TopMovingProducts { get; set; } = new List<TopMovingProductDTO>();
    }
}
