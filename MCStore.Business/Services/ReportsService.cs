using MCStore.Business.IServices;
using MCStore.DAL.IRepositories;
using MCStore.DAL.Repositories;
using MCStore.Domain.DTO;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Business.Services
{
    public class ReportsService: IReportsService
    {
        public readonly IReportsRepository _reportsRepository;

        public ReportsService(IReportsRepository reportsRepository)
        {
            _reportsRepository = reportsRepository;
        }

        public async Task<SalesReportDTO> GetSalesReportAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _reportsRepository.GetSalesReportAsync(startDate, endDate);
        }

        public async Task<StockInsightsDTO> GetStockInsightsAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _reportsRepository.GetStockInsightsAsync(startDate, endDate);
        }
    }
}
