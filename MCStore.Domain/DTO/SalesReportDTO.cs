using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Domain.DTO
{
    public class SalesReportDTO
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal NetSalesNormal { get; set; }

        public decimal CostOfNormalProductSold { get; set; }

        public decimal Margin { get; set; }

        public decimal NetSalesDigital { get; set; }

        public decimal Profit { get; set; }
    }
}
