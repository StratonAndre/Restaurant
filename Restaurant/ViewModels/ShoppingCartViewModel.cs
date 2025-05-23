using RestaurantManager.Models;
using RestaurantManager.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RestaurantManager.Utilities;

namespace RestaurantManager.ViewModels
{
    public class ShoppingCartViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly ConfigurationService _configurationService;
        
        private ObservableCollection<CartItem> _cartItems;
        private decimal _subtotal;
        private decimal _deliveryCost;
        private decimal _discount;
        private decimal _total;
        private bool _isPlacingOrder;
        private bool _isCartEmpty => CartItems.Count == 0;

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set => SetProperty(ref _cartItems, value);
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set => SetProperty(ref _subtotal, value);
        }

        public decimal DeliveryCost
        {
            get => _deliveryCost;
            set => SetProperty(ref _deliveryCost, value);
        }

        public decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        public decimal Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public bool IsPlacingOrder
        {
            get => _isPlacingOrder;
            set => SetProperty(ref _isPlacingOrder, value);
        }

        public bool IsCartEmpty
        {
            get => _isCartEmpty;
            set => OnPropertyChanged(nameof(IsCartEmpty));
        }

        public User CurrentUser => _userService.CurrentUser;
        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        public bool IsClient => _userService.IsClient;

        public ICommand IncreaseQuantityCommand { get; private set; }
        public ICommand DecreaseQuantityCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ICommand ClearCartCommand { get; private set; }
        public ICommand PlaceOrderCommand { get; private set; }

        // Event raised when an order is successfully placed
        public event EventHandler<Order> OrderPlaced;

        public ShoppingCartViewModel(UserService userService, OrderService orderService, ConfigurationService configurationService)
        {
            _userService = userService;
            _orderService = orderService;
            _configurationService = configurationService;
            
            Title = "Shopping Cart";
            CartItems = new ObservableCollection<CartItem>();

            // Set up collection changed event to update totals
            CartItems.CollectionChanged += (s, e) => {
                CalculateTotals();
                OnPropertyChanged(nameof(IsCartEmpty));
            };

            IncreaseQuantityCommand = new RelayCommand<CartItem>(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand<CartItem>(DecreaseQuantity);
            RemoveItemCommand = new RelayCommand<CartItem>(RemoveItem);
            ClearCartCommand = new RelayCommand(ClearCart);
            PlaceOrderCommand = new RelayCommand(async () => await PlaceOrderAsync(), CanPlaceOrder);
        }

        // Add an item to the cart
        public void AddItem(MenuItem menuItem, int quantity = 1)
        {
            if (menuItem == null || quantity <= 0 || !menuItem.IsAvailable)
                return;

            // Check if the item already exists in the cart
            var existingItem = CartItems.FirstOrDefault(i => 
                (menuItem is Dish && i.MenuItem is Dish && ((Dish)i.MenuItem).Id == ((Dish)menuItem).Id) ||
                (menuItem is Menu && i.MenuItem is Menu && ((Menu)i.MenuItem).Id == ((Menu)menuItem).Id));

            if (existingItem != null)
            {
                // Increase the quantity of the existing item
                existingItem.Quantity += quantity;
            }
            else
            {
                // Add a new item to the cart
                var cartItem = new CartItem(menuItem, quantity);
                
                // Subscribe to property changes to update totals
                cartItem.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.UnitPrice))
                    {
                        CalculateTotals();
                    }
                };
                
                CartItems.Add(cartItem);
            }

            // Update totals
            CalculateTotals();
        }

        // Increase the quantity of an item in the cart
        private void IncreaseQuantity(CartItem item)
        {
            if (item != null)
            {
                item.Quantity++;
                CalculateTotals();
            }
        }

        // Decrease the quantity of an item in the cart
        private void DecreaseQuantity(CartItem item)
        {
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                CalculateTotals();
            }
        }

        // Remove an item from the cart
        private void RemoveItem(CartItem item)
        {
            if (item != null)
            {
                CartItems.Remove(item);
                CalculateTotals();
            }
        }

        // Clear the cart
        public void ClearCart()
        {
            CartItems.Clear();
            CalculateTotals();
        }

        // Calculate totals (subtotal, delivery cost, discount, total)
        private void CalculateTotals()
        {
            // Calculate subtotal
            Subtotal = CartItems.Sum(i => i.TotalPrice);
            
            // Calculate delivery cost based on order value and configuration
            decimal minOrderForFreeDelivery = _configurationService.MinOrderForFreeDelivery;
            decimal standardDeliveryCost = _configurationService.StandardDeliveryCost;
            DeliveryCost = Subtotal >= minOrderForFreeDelivery ? 0 : standardDeliveryCost;
            
            // Calculate potential discount
            decimal orderDiscountThreshold = _configurationService.OrderDiscountThreshold;
            decimal orderDiscountPercentage = _configurationService.OrderDiscountPercentage;
            Discount = Subtotal >= orderDiscountThreshold ? Subtotal * (orderDiscountPercentage / 100) : 0;
            
            // Calculate total
            Total = Subtotal + DeliveryCost - Discount;
            
            // Update IsCartEmpty
            OnPropertyChanged(nameof(IsCartEmpty));
        }

        // Check if an order can be placed
        private bool CanPlaceOrder()
        {
            return IsClient && !IsCartEmpty && !IsPlacingOrder;
        }

        // Place an order
        private async Task PlaceOrderAsync()
        {
            if (!CanPlaceOrder())
                return;

            try
            {
                IsPlacingOrder = true;
                ErrorMessage = null;
                
                // Create a new order
                var order = new Order
                {
                    UserId = CurrentUser.UserId,
                    OrderDate = DateTime.Now,
                    EstimatedDeliveryTime = DateTime.Now.AddHours(1),
                    StatusId = (int)OrderStatusType.Registered,
                    FoodCost = Subtotal,
                    DeliveryCost = DeliveryCost,
                    DiscountAmount = Discount
                };
                
                // Add order details from the cart items
                foreach (var item in CartItems)
                {
                    var detail = new OrderDetail
                    {
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    
                    // Set either DishId or MenuId based on the item type
                    if (item.MenuItem is Dish dish)
                    {
                        detail.DishId = dish.Id;
                    }
                    else if (item.MenuItem is Menu menu)
                    {
                        detail.MenuId = menu.Id;
                    }
                    
                    order.OrderDetails.Add(detail);
                }
                
                // Place the order
                var placedOrder = await _orderService.PlaceOrderAsync(order);
                
                if (placedOrder != null)
                {
                    // Clear the cart
                    ClearCart();
                    
                    // Notify successful order placement
                    OrderPlaced?.Invoke(this, placedOrder);
                    
                    ErrorMessage = $"Order #{placedOrder.OrderCode} placed successfully!";
                }
                else
                {
                    ErrorMessage = "Failed to place order. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to place order: {ex.Message}";
            }
            finally
            {
                IsPlacingOrder = false;
            }
        }
    }
}