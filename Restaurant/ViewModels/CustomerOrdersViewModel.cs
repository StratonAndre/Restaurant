using RestaurantManager.Models;
using RestaurantManager.Services;
using RestaurantManager.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantManager.ViewModels
{
    public class CustomerOrdersViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        
        private ObservableCollection<Order> _allOrders;
        private ObservableCollection<Order> _activeOrders;
        private Order _selectedOrder;
        private bool _isLoadingData;

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
            set => SetProperty(ref _selectedOrder, value);
        }

        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }

        public User CurrentUser => _userService.CurrentUser;
        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        public bool IsClient => _userService.IsClient;

        public ICommand RefreshCommand { get; private set; }
        public ICommand CancelOrderCommand { get; private set; }
        public ICommand ViewOrderDetailsCommand { get; private set; }

        // Event raised when the user wants to view order details
        public event EventHandler<Order> ViewOrderDetailsRequested;

        public CustomerOrdersViewModel(UserService userService, OrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
            
            Title = "My Orders";
            AllOrders = new ObservableCollection<Order>();
            ActiveOrders = new ObservableCollection<Order>();

            RefreshCommand = new RelayCommand(async () => await LoadOrdersAsync());
            CancelOrderCommand = new RelayCommand<Order>(async (order) => await CancelOrderAsync(order), CanCancelOrder);
            ViewOrderDetailsCommand = new RelayCommand<Order>(OnViewOrderDetailsRequested);
            
            // Initial data load
            LoadOrdersAsync().ConfigureAwait(false);
        }

        private async Task LoadOrdersAsync()
        {
            if (!IsClient)
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Load all orders for the current user
                var allOrders = await _orderService.GetOrdersForUserAsync(CurrentUser.UserId);
                AllOrders = new ObservableCollection<Order>(allOrders.OrderByDescending(o => o.OrderDate));
                
                // Filter active orders
                var activeOrders = await _orderService.GetActiveOrdersForUserAsync(CurrentUser.UserId);
                ActiveOrders = new ObservableCollection<Order>(activeOrders.OrderByDescending(o => o.OrderDate));
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

        private bool CanCancelOrder(Order order)
        {
            return order != null && order.IsActive && 
                  (order.StatusId == (int)OrderStatusType.Registered || order.StatusId == (int)OrderStatusType.InPreparation);
        }

        private async Task CancelOrderAsync(Order order)
        {
            if (!CanCancelOrder(order))
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Cancel the order
                bool success = await _orderService.CancelOrderAsync(order.OrderId);
                
                if (success)
                {
                    // Refresh the orders
                    await LoadOrdersAsync();
                }
                else
                {
                    ErrorMessage = "Failed to cancel order. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to cancel order: {ex.Message}";
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