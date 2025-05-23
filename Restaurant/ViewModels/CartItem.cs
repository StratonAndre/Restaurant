using RestaurantManager.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.ViewModels
{
    /// <summary>
    /// Represents an item in the shopping cart.
    /// </summary>
    public class CartItem : INotifyPropertyChanged
    {
        private MenuItem _menuItem;
        private int _quantity;
        private decimal _unitPrice;

        /// <summary>
        /// Gets or sets the menu item (dish or menu).
        /// </summary>
        public MenuItem MenuItem
        {
            get => _menuItem;
            set
            {
                if (_menuItem != value)
                {
                    _menuItem = value;
                    if (value != null)
                    {
                        UnitPrice = value.Price;
                        OnPropertyChanged(nameof(ItemName));
                        OnPropertyChanged(nameof(ItemType));
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the quantity of the item.
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
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        /// <summary>
        /// Gets or sets the unit price of the item.
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
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        /// <summary>
        /// Gets the total price for this cart item (quantity * unit price).
        /// </summary>
        public decimal TotalPrice => Quantity * UnitPrice;

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string ItemName => MenuItem?.Name ?? "Unknown Item";

        /// <summary>
        /// Gets the type of the item ("Dish" or "Menu").
        /// </summary>
        public string ItemType => MenuItem is Dish ? "Dish" : (MenuItem is Menu ? "Menu" : "Unknown");

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItem"/> class.
        /// </summary>
        public CartItem()
        {
            Quantity = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItem"/> class with the specified menu item and quantity.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <param name="quantity">The quantity.</param>
        public CartItem(MenuItem menuItem, int quantity = 1)
        {
            MenuItem = menuItem;
            Quantity = quantity;
        }

        #region INotifyPropertyChanged Implementation
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}