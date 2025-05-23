using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    /// <summary>
    /// Service for generating reports.
    /// </summary>
    public class ReportService
    {
        private readonly DatabaseService _databaseService;
        private readonly OrderService _orderService;
        private readonly DishService _dishService;
        private readonly ConfigurationService _configurationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportService"/> class.
        /// </summary>
        /// <param name="databaseService">The database service.</param>
        /// <param name="orderService">The order service.</param>
        /// <param name="dishService">The dish service.</param>
        /// <param name="configurationService">The configuration service.</param>
        public ReportService(DatabaseService databaseService, OrderService orderService, 
            DishService dishService, ConfigurationService configurationService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        }

        /// <summary>
        /// Gets the sales report for a specific date range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>A dataset containing the sales report.</returns>
        public async Task<DataSet> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "StartDate", startDate },
                    { "EndDate", endDate }
                };

                return await _databaseService.ExecuteDataSetAsync("sp_GetSalesReport", parameters);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"GetSalesReportAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the popular dishes report.
        /// </summary>
        /// <param name="count">The number of popular dishes to retrieve.</param>
        /// <returns>A dataset containing the popular dishes report.</returns>
        public async Task<DataSet> GetPopularDishesReportAsync(int count = 10)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Count", count }
                };

                return await _databaseService.ExecuteDataSetAsync("sp_GetPopularDishes", parameters);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"GetPopularDishesReportAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the low stock report.
        /// </summary>
        /// <returns>A list of dishes with low stock.</returns>
        public async Task<List<Dish>> GetLowStockReportAsync()
        {
            try
            {
                decimal threshold = _configurationService.LowStockThreshold;
                return (await _dishService.GetLowStockDishesAsync(threshold)).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"GetLowStockReportAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the revenue by category report.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>A dataset containing the revenue by category report.</returns>
        public async Task<DataSet> GetRevenueByCategoryReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "StartDate", startDate },
                    { "EndDate", endDate }
                };

                return await _databaseService.ExecuteDataSetAsync("sp_GetRevenueByCategory", parameters);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"GetRevenueByCategoryReportAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Exports a report to a CSV file.
        /// </summary>
        /// <param name="dataTable">The data table to export.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>True if the export was successful, false otherwise.</returns>
        public bool ExportToCsv(DataTable dataTable, string filePath)
        {
            try
            {
                var sb = new StringBuilder();

                // Add column headers
                var columnNames = dataTable.Columns.Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToArray();
                sb.AppendLine(string.Join(",", columnNames));

                // Add rows
                foreach (DataRow row in dataTable.Rows)
                {
                    var fields = row.ItemArray
                        .Select(field => field != null 
                            ? $"\"{field.ToString().Replace("\"", "\"\"")}\""
                            : "\"\"")
                        .ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }

                // Save to file
                File.WriteAllText(filePath, sb.ToString());
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"ExportToCsv error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the customer order history report.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of orders for the specified user.</returns>
        public async Task<List<Order>> GetCustomerOrderHistoryReportAsync(int userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersForUserAsync(userId);
                return orders.ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"GetCustomerOrderHistoryReportAsync error: {ex.Message}");
                throw;
            }
        }
    }
}