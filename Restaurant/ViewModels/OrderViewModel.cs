using RestaurantManager.Models;
using RestaurantManager.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using RestaurantManager.Utilities;
namespace RestaurantManager.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        
        private Order _order;
        private ObservableCollection<OrderDetail> _orderDetails;
        private bool _isLoadingData;
        private bool _isCancelling;

        public Order Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set => SetProperty(ref _orderDetails, value);
        }

        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }

        public bool IsCancelling
        {
            get => _isCancelling;
            set => SetProperty(ref _isCancelling, value);
        }

        public User CurrentUser => _userService.CurrentUser;
        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        public bool IsClient => _userService.IsClient;
        public bool IsEmployee => _userService.IsEmployee;
        
        // Only clients can cancel their own orders, and only active orders can be cancelled
        public bool CanCancelOrder => IsClient && Order != null && Order.IsActive && !IsCancelling;
        
        // Only employees can update order status
        public bool CanUpdateStatus => IsEmployee && Order != null && Order.IsActive;

        public ICommand CancelOrderCommand { get; private set; }
        public ICommand UpdateStatusCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        // Event raised when the order is cancelled
        public event EventHandler OrderCancelled;
        
        // Event raised when the order status is updated
        public event EventHandler OrderStatusUpdated;

        public OrderViewModel(UserService userService, OrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
            
            Title = "Order Details";
            OrderDetails = new ObservableCollection<OrderDetail>();

            CancelOrderCommand = new RelayCommand(async () => await CancelOrderAsync(), () => CanCancelOrder);
            UpdateStatusCommand = new RelayCommand<OrderStatus>(async (status) => await UpdateOrderStatusAsync(status), (status) => CanUpdateStatus);
            RefreshCommand = new RelayCommand(async () => await LoadOrderAsync());
        }

        /// <summary>
        /// Load an order by ID
        /// </summary>
        public async Task LoadOrderAsync(int orderId)
        {
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                var order = await _orderService.GetOrderByIdAsync(orderId);
                
                if (order != null)
                {
                    // Check if the user has access to this order
                    if (IsClient && order.UserId != CurrentUser.UserId)
                    {
                        ErrorMessage = "You do not have permission to view this order.";
                        return;
                    }
                    
                    Order = order;
                    OrderDetails = new ObservableCollection<OrderDetail>(order.OrderDetails);
                    Title = $"Order #{order.OrderCode}";
                }
                else
                {
                    ErrorMessage = "Order not found.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load order: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        /// <summary>
        /// Reload the current order
        /// </summary>
        private async Task LoadOrderAsync()
        {
            if (Order != null)
            {
                await LoadOrderAsync(Order.OrderId);
            }
        }

        /// <summary>
        /// Cancel the current order
        /// </summary>
        private async Task CancelOrderAsync()
        {
            if (!CanCancelOrder)
                return;

            try
            {
                IsCancelling = true;
                ErrorMessage = null;
                
                bool success = await _orderService.CancelOrderAsync(Order.OrderId);
                
                if (success)
                {
                    // Reload the order to get the updated status
                    await LoadOrderAsync();
                    
                    // Notify the order has been cancelled
                    OrderCancelled?.Invoke(this, EventArgs.Empty);
                    
                    ErrorMessage = "Order cancelled successfully.";
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
                IsCancelling = false;
            }
        }

        /// <summary>
        /// Update the order status
        /// </summary>
        private async Task UpdateOrderStatusAsync(OrderStatus newStatus)
        {
            if (!CanUpdateStatus || newStatus == null)
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                bool success = await _orderService.UpdateOrderStatusAsync(Order.OrderId, (OrderStatusType)newStatus.StatusId);
                
                if (success)
                {
                    // Reload the order to get the updated status
                    await LoadOrderAsync();
                    
                    // Notify the order status has been updated
                    OrderStatusUpdated?.Invoke(this, EventArgs.Empty);
                    
                    ErrorMessage = $"Order status updated to {newStatus.StatusName}.";
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
    }
}