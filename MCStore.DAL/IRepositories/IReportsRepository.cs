using MCStore.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.IRepositories
{
    public interface IReportsRepository
    {
        Task<SalesReportDTO> GetSalesReportAsync(DateTime? startDate, DateTime? endDate);
        Task<StockInsightsDTO> GetStockInsightsAsync(DateTime? startDate, DateTime? endDate);
    }
}
