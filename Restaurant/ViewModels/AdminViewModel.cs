using RestaurantManager.Models;
using RestaurantManager.Services;
using RestaurantManager.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Threading.Tasks;
using System.Linq;

namespace RestaurantManager.ViewModels
{
    /// <summary>
    /// ViewModel for the admin dashboard.
    /// </summary>
    public class AdminViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly DishService _dishService;
        private readonly ReportService _reportService;
        private readonly ConfigurationService _configurationService;
        
        private int _activeOrders;
        private decimal _todayRevenue;
        private decimal _yesterdayRevenue;
        private double _revenueChange;
        private SolidColorBrush _revenueChangeColor;
        private int _lowStockCount;
        private ObservableCollection<Dish> _lowStockItems;
        private bool _isLoadingData;
        
        /// <summary>
        /// Gets or sets the number of active orders.
        /// </summary>
        public int ActiveOrders
        {
            get => _activeOrders;
            set => SetProperty(ref _activeOrders, value);
        }
        
        /// <summary>
        /// Gets or sets the today's revenue.
        /// </summary>
        public decimal TodayRevenue
        {
            get => _todayRevenue;
            set => SetProperty(ref _todayRevenue, value);
        }
        
        /// <summary>
        /// Gets or sets the revenue change percentage.
        /// </summary>
        public double RevenueChange
        {
            get => _revenueChange;
            set => SetProperty(ref _revenueChange, value);
        }
        
        /// <summary>
        /// Gets or sets the revenue change color.
        /// </summary>
        public SolidColorBrush RevenueChangeColor
        {
            get => _revenueChangeColor;
            set => SetProperty(ref _revenueChangeColor, value);
        }
        
        /// <summary>
        /// Gets or sets the number of low stock items.
        /// </summary>
        public int LowStockCount
        {
            get => _lowStockCount;
            set => SetProperty(ref _lowStockCount, value);
        }
        
        /// <summary>
        /// Gets or sets the low stock items.
        /// </summary>
        public ObservableCollection<Dish> LowStockItems
        {
            get => _lowStockItems;
            set => SetProperty(ref _lowStockItems, value);
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether data is being loaded.
        /// </summary>
        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }
        
        /// <summary>
        /// Gets the current user.
        /// </summary>
        public User CurrentUser => _userService.CurrentUser;
        
        /// <summary>
        /// Gets a value indicating whether the user is authenticated.
        /// </summary>
        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        
        /// <summary>
        /// Gets a value indicating whether the user is an employee.
        /// </summary>
        public bool IsEmployee => _userService.IsEmployee;
        
        /// <summary>
        /// Gets the command to navigate to the orders view.
        /// </summary>
        public ICommand NavigateToOrdersCommand { get; }
        
        /// <summary>
        /// Gets the command to navigate to the dishes view.
        /// </summary>
        public ICommand NavigateToDishesCommand { get; }
        
        /// <summary>
        /// Gets the command to navigate to the menus view.
        /// </summary>
        public ICommand NavigateToMenusCommand { get; }
        
        /// <summary>
        /// Gets the command to view low stock items.
        /// </summary>
        public ICommand ViewLowStockCommand { get; }
        
        /// <summary>
        /// Gets the command to generate a report.
        /// </summary>
        public ICommand GenerateReportCommand { get; }
        
        /// <summary>
        /// Gets the command to edit a dish.
        /// </summary>
        public ICommand EditDishCommand { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminViewModel"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="orderService">The order service.</param>
        /// <param name="dishService">The dish service.</param>
        /// <param name="reportService">The report service.</param>
        /// <param name="configurationService">The configuration service.</param>
        public AdminViewModel(UserService userService, OrderService orderService, 
            DishService dishService, ReportService reportService, ConfigurationService configurationService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            
            Title = "Admin Dashboard";
            LowStockItems = new ObservableCollection<Dish>();
            RevenueChangeColor = new SolidColorBrush(Colors.Green);
            
            NavigateToOrdersCommand = new RelayCommand(NavigateToOrders);
            NavigateToDishesCommand = new RelayCommand(NavigateToDishes);
            NavigateToMenusCommand = new RelayCommand(NavigateToMenus);
            ViewLowStockCommand = new RelayCommand(ViewLowStock);
            GenerateReportCommand = new RelayCommand(GenerateReport);
            EditDishCommand = new RelayCommand<Dish>(EditDish);
            
            // Load data
            LoadDataAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Loads dashboard data.
        /// </summary>
        private async Task LoadDataAsync()
        {
            if (!IsEmployee)
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                
                // Get order statistics
                var statistics = await _orderService.GetOrderStatisticsAsync();
                
                // Update active orders
                ActiveOrders = statistics.ContainsKey("ActiveOrderCount") ? Convert.ToInt32(statistics["ActiveOrderCount"]) : 0;
                
                // Update today's revenue
                TodayRevenue = statistics.ContainsKey("TodayRevenue") ? Convert.ToDecimal(statistics["TodayRevenue"]) : 0;
                
                // Update yesterday's revenue
                _yesterdayRevenue = statistics.ContainsKey("YesterdayRevenue") ? Convert.ToDecimal(statistics["YesterdayRevenue"]) : 0;
                
                // Calculate revenue change
                if (_yesterdayRevenue > 0)
                {
                    RevenueChange = (double)((TodayRevenue - _yesterdayRevenue) / _yesterdayRevenue);
                    RevenueChangeColor = new SolidColorBrush(RevenueChange >= 0 ? Colors.Green : Colors.Red);
                }
                else
                {
                    RevenueChange = 0;
                    RevenueChangeColor = new SolidColorBrush(Colors.Gray);
                }
                
                // Get low stock items
                var lowStockThreshold = _configurationService.LowStockThreshold;
                var lowStockItems = await _dishService.GetLowStockDishesAsync(lowStockThreshold);
                
                LowStockItems.Clear();
                foreach (var item in lowStockItems)
                {
                    LowStockItems.Add(item);
                }
                
                LowStockCount = LowStockItems.Count;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load dashboard data: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Navigates to the orders view.
        /// </summary>
        private void NavigateToOrders()
        {
            // This will be implemented by the parent view model
            // or navigation service
        }
        
        /// <summary>
        /// Navigates to the dishes view.
        /// </summary>
        private void NavigateToDishes()
        {
            // This will be implemented by the parent view model
            // or navigation service
        }
        
        /// <summary>
        /// Navigates to the menus view.
        /// </summary>
        private void NavigateToMenus()
        {
            // This will be implemented by the parent view model
            // or navigation service
        }
        
        /// <summary>
        /// Views low stock items.
        /// </summary>
        private void ViewLowStock()
        {
            // This will be implemented by the parent view model
            // or navigation service
        }
        
        /// <summary>
        /// Generates a report.
        /// </summary>
        private async void GenerateReport()
        {
            try
            {
                IsLoadingData = true;
                
                // Get the sales report for the current month
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = DateTime.Now;
                
                var salesReport = await _reportService.GetSalesReportAsync(startDate, endDate);
                
                // Export to CSV
                var fileName = $"SalesReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv";
                var filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
                
                if (salesReport != null && salesReport.Tables.Count > 0)
                {
                    bool success = _reportService.ExportToCsv(salesReport.Tables[0], filePath);
                    
                    if (success)
                    {
                        MessageBox.Show($"Report has been exported to {filePath}", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to export the report", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No data available for the report", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to generate report: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Edits a dish.
        /// </summary>
        /// <param name="dish">The dish to edit.</param>
        private void EditDish(Dish dish)
        {
            if (dish == null)
            {
                return;
            }
            
            // This will be implemented by the parent view model
            // or navigation service
            // For example: NavigationService.Navigate(new DishManagementView(dish));
        }
    }
}