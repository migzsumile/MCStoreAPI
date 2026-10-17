using MCStore.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.IServices
{
    public interface IReportsService
    {
        Task<SalesReportDTO> GetSalesReportAsync(DateTime? startDate, DateTime? endDate);
        Task<StockInsightsDTO> GetStockInsightsAsync(DateTime? startDate, DateTime? endDate);
    }
}
