using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace RestaurantManager.Models
{
    public class Menu : MenuItem
    {
        private int _menuId;
        private decimal _discountPercentage;
        private ObservableCollection<MenuDish>? _menuDishes;

        public override int Id
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

        public decimal DiscountPercentage
        {
            get => _discountPercentage;
            set
            {
                if (_discountPercentage != value)
                {
                    _discountPercentage = value;
                    CalculatePrice();
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<MenuDish>? MenuDishes
        {
            get => _menuDishes;
            set
            {
                if (_menuDishes != value)
                {
                    if (_menuDishes != null)
                    {
                        // Detach event handlers from old collection
                        foreach (var menuDish in _menuDishes)
                        {
                            if (menuDish != null)
                                menuDish.PropertyChanged -= MenuDish_PropertyChanged;
                        }
                        _menuDishes.CollectionChanged -= MenuDishes_CollectionChanged;
                    }

                    _menuDishes = value;

                    if (_menuDishes != null)
                    {
                        // Attach event handlers to new collection
                        foreach (var menuDish in _menuDishes)
                        {
                            if (menuDish != null)
                                menuDish.PropertyChanged += MenuDish_PropertyChanged;
                        }
                        _menuDishes.CollectionChanged += MenuDishes_CollectionChanged;
                    }

                    CalculatePrice();
                    CheckAvailability();
                    OnPropertyChanged();
                }
            }
        }

        public decimal OriginalPrice { get; private set; }

        public Menu()
        {
            MenuDishes = new ObservableCollection<MenuDish>();
            IsAvailable = true;
        }

        // Event handlers for MenuDishes collection changes
        private void MenuDishes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MenuDish menuDish in e.NewItems)
                {
                    if (menuDish != null)
                        menuDish.PropertyChanged += MenuDish_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (MenuDish menuDish in e.OldItems)
                {
                    if (menuDish != null)
                        menuDish.PropertyChanged -= MenuDish_PropertyChanged;
                }
            }

            CalculatePrice();
            CheckAvailability();
        }

        private void MenuDish_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MenuDish.Dish) || 
                e.PropertyName == nameof(MenuDish.CustomQuantity))
            {
                CalculatePrice();
                CheckAvailability();
            }
        }

        // Calculate the menu price based on included dishes and discount
        private void CalculatePrice()
        {
            if (MenuDishes == null || MenuDishes.Count == 0)
            {
                Price = 0;
                OriginalPrice = 0;
                return;
            }

            decimal totalPrice = MenuDishes.Sum(md => md.Dish?.Price ?? 0);
            OriginalPrice = totalPrice;
            
            // Apply discount
            Price = totalPrice - (totalPrice * DiscountPercentage / 100);
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(OriginalPrice));
        }

        // Check if all dishes in the menu are available
        private void CheckAvailability()
        {
            if (MenuDishes == null || MenuDishes.Count == 0)
            {
                IsAvailable = false;
                return;
            }

            IsAvailable = MenuDishes.All(md => md.Dish?.IsAvailable ?? false);
        }
    }
}