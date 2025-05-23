using RestaurantManager.Models;
using RestaurantManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RestaurantManager.Utilities;
namespace RestaurantManager.ViewModels
{
    public class OrderManagementViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        
        private ObservableCollection<Order> _allOrders;
        private ObservableCollection<Order> _activeOrders;
        private Order _selectedOrder;
        private ObservableCollection<OrderStatus> _orderStatuses;
        private OrderStatus _selectedOrderStatus;
        private bool _isLoadingData;
        private Dictionary<string, object> _orderStatistics;

        public ObservableCollection<Order> AllOrders
        {
            get => _allOrders;
            set => SetProperty(ref _allOrders, value);
        }

        public ObservableCollection<Order> ActiveOrders
        {
            get => _activeOrders;
            set => SetProperty(ref _activeOrders, value);
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (SetProperty(ref _selectedOrder, value) && value != null)
                {
                    // Update the selected status
                    SelectedOrderStatus = OrderStatuses.FirstOrDefault(s => s.StatusId == value.StatusId);
                }
            }
        }

        public ObservableCollection<OrderStatus> OrderStatuses
        {
            get => _orderStatuses;
            set => SetProperty(ref _orderStatuses, value);
        }

        public OrderStatus SelectedOrderStatus
        {
            get => _selectedOrderStatus;
            set => SetProperty(ref _selectedOrderStatus, value);
        }

        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }

        public Dictionary<string, object> OrderStatistics
        {
            get => _orderStatistics;
            set => SetProperty(ref _orderStatistics, value);
        }

        // Statistics properties
        public int TotalOrders => OrderStatistics != null && OrderStatistics.ContainsKey("TotalOrders") ? 
                                Convert.ToInt32(OrderStatistics["TotalOrders"]) : 0;
                                
        public int ActiveOrderCount => OrderStatistics != null && OrderStatistics.ContainsKey("ActiveOrderCount") ? 
                                     Convert.ToInt32(OrderStatistics["ActiveOrderCount"]) : 0;
                                     
        public decimal TodayRevenue => OrderStatistics != null && OrderStatistics.ContainsKey("TodayRevenue") ? 
                                     Convert.ToDecimal(OrderStatistics["TodayRevenue"]) : 0;
                                     
        public decimal WeekRevenue => OrderStatistics != null && OrderStatistics.ContainsKey("WeekRevenue") ? 
                                    Convert.ToDecimal(OrderStatistics["WeekRevenue"]) : 0;
                                    
        public decimal MonthRevenue => OrderStatistics != null && OrderStatistics.ContainsKey("MonthRevenue") ? 
                                     Convert.ToDecimal(OrderStatistics["MonthRevenue"]) : 0;

        public User CurrentUser => _userService.CurrentUser;
        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        public bool IsEmployee => _userService.IsEmployee;

        public ICommand RefreshCommand { get; private set; }
        public ICommand UpdateOrderStatusCommand { get; private set; }
        public ICommand ViewOrderDetailsCommand { get; private set; }

        // Event raised when the user wants to view order details
        public event EventHandler<Order> ViewOrderDetailsRequested;

        public OrderManagementViewModel(UserService userService, OrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
            
            Title = "Order Management";
            AllOrders = new ObservableCollection<Order>();
            ActiveOrders = new ObservableCollection<Order>();
            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderStatistics = new Dictionary<string, object>();

            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            UpdateOrderStatusCommand = new RelayCommand(async () => await UpdateOrderStatusAsync(), CanUpdateOrderStatus);
            ViewOrderDetailsCommand = new RelayCommand<Order>(OnViewOrderDetailsRequested);
            
            // Initial data load
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            if (!IsEmployee)
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Load all orders
                var allOrders = await _orderService.GetAllOrdersAsync();
                AllOrders = new ObservableCollection<Order>(allOrders.OrderByDescending(o => o.OrderDate));
                
                // Load active orders
                var activeOrders = await _orderService.GetActiveOrdersAsync();
                ActiveOrders = new ObservableCollection<Order>(activeOrders.OrderByDescending(o => o.OrderDate));
                
                // Load order statuses
                LoadOrderStatuses();
                
                // Load order statistics
                OrderStatistics = await _orderService.GetOrderStatisticsAsync();
                
                // Update statistics properties
                OnPropertyChanged(nameof(TotalOrders));
                OnPropertyChanged(nameof(ActiveOrderCount));
                OnPropertyChanged(nameof(TodayRevenue));
                OnPropertyChanged(nameof(WeekRevenue));
                OnPropertyChanged(nameof(MonthRevenue));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load orders: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private void LoadOrderStatuses()
        {
            OrderStatuses.Clear();
            
            // Add all possible order statuses
            OrderStatuses.Add(new OrderStatus { StatusId = (int)OrderStatusType.Registered, StatusName = "Registered" });
            OrderStatuses.Add(new OrderStatus { StatusId = (int)OrderStatusType.InPreparation, StatusName = "In Preparation" });
            OrderStatuses.Add(new OrderStatus { StatusId = (int)OrderStatusType.OutForDelivery, StatusName = "Out for Delivery" });
            OrderStatuses.Add(new OrderStatus { StatusId = (int)OrderStatusType.Delivered, StatusName = "Delivered" });
            OrderStatuses.Add(new OrderStatus { StatusId = (int)OrderStatusType.Cancelled, StatusName = "Cancelled" });
        }

        private bool CanUpdateOrderStatus()
        {
            return SelectedOrder != null && SelectedOrderStatus != null && 
                   SelectedOrder.StatusId != SelectedOrderStatus.StatusId && SelectedOrder.IsActive;
        }

        private async Task UpdateOrderStatusAsync()
        {
            if (!CanUpdateOrderStatus())
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Update the order status
                bool success = await _orderService.UpdateOrderStatusAsync(
                    SelectedOrder.OrderId, (OrderStatusType)SelectedOrderStatus.StatusId);
                
                if (success)
                {
                    // Refresh the orders
                    await LoadDataAsync();
                }
                else
                {
                    ErrorMessage = "Failed to update order status. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update order status: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private void OnViewOrderDetailsRequested(Order order)
        {
            if (order != null)
            {
                ViewOrderDetailsRequested?.Invoke(this, order);
            }
        }
    }
}