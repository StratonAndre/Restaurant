using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    /// <summary>
    /// Represents an order detail (line item) in an order.
    /// </summary>
    public class OrderDetail : INotifyPropertyChanged
    {
        private int _orderDetailId;
        private int _orderId;
        private int? _dishId;
        private int? _menuId;
        private int _quantity;
        private decimal _unitPrice;
        private Order _order;
        private Dish _dish;
        private Menu _menu;

        /// <summary>
        /// Gets or sets the order detail ID.
        /// </summary>
        public int OrderDetailId
        {
            get => _orderDetailId;
            set
            {
                if (_orderDetailId != value)
                {
                    _orderDetailId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the order ID.
        /// </summary>
        public int OrderId
        {
            get => _orderId;
            set
            {
                if (_orderId != value)
                {
                    _orderId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the dish ID (nullable, if this detail is for a menu).
        /// </summary>
        public int? DishId
        {
            get => _dishId;
            set
            {
                if (_dishId != value)
                {
                    _dishId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the menu ID (nullable, if this detail is for a dish).
        /// </summary>
        public int? MenuId
        {
            get => _menuId;
            set
            {
                if (_menuId != value)
                {
                    _menuId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    // Also notify that LineTotal has changed
                    OnPropertyChanged(nameof(LineTotal));
                }
            }
        }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged();
                    // Also notify that LineTotal has changed
                    OnPropertyChanged(nameof(LineTotal));
                }
            }
        }

        /// <summary>
        /// Gets or sets the order this detail belongs to.
        /// </summary>
        public Order Order
        {
            get => _order;
            set
            {
                if (_order != value)
                {
                    _order = value;
                    OrderId = value?.OrderId ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the dish for this detail (if applicable).
        /// </summary>
        public Dish Dish
        {
            get => _dish;
            set
            {
                if (_dish != value)
                {
                    _dish = value;
                    DishId = value?.Id;
                    
                    if (value != null)
                    {
                        // If setting a dish, clear the menu
                        MenuId = null;
                        Menu = null;
                        // Update the unit price
                        UnitPrice = value.Price;
                    }
                    
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ItemName));
                    OnPropertyChanged(nameof(ItemType));
                    OnPropertyChanged(nameof(MenuItem));
                }
            }
        }

        /// <summary>
        /// Gets or sets the menu for this detail (if applicable).
        /// </summary>
        public Menu Menu
        {
            get => _menu;
            set
            {
                if (_menu != value)
                {
                    _menu = value;
                    MenuId = value?.Id;
                    
                    if (value != null)
                    {
                        // If setting a menu, clear the dish
                        DishId = null;
                        Dish = null;
                        // Update the unit price
                        UnitPrice = value.Price;
                    }
                    
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ItemName));
                    OnPropertyChanged(nameof(ItemType));
                    OnPropertyChanged(nameof(MenuItem));
                }
            }
        }

        /// <summary>
        /// Gets the total price for this line item.
        /// </summary>
        public decimal LineTotal => Quantity * UnitPrice;

        /// <summary>
        /// Gets the name of the item (dish or menu).
        /// </summary>
        public string ItemName => Dish?.Name ?? Menu?.Name ?? "Unknown Item";

        /// <summary>
        /// Gets the type of the item ("Dish" or "Menu").
        /// </summary>
        public string ItemType => Dish != null ? "Dish" : (Menu != null ? "Menu" : "Unknown");

        /// <summary>
        /// Gets the menu item (dish or menu).
        /// </summary>
        public MenuItem MenuItem => (MenuItem)Dish ?? Menu;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Called when a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}