using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public class MenuDish : INotifyPropertyChanged
    {
        private int _menuDishId;
        private int _menuId;
        private int _dishId;
        private int _customQuantity; // In grams, different from the dish's standard portion
        private Menu _menu;
        private Dish _dish;

        public int MenuDishId
        {
            get => _menuDishId;
            set
            {
                if (_menuDishId != value)
                {
                    _menuDishId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MenuId
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

        public int DishId
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

        public int CustomQuantity
        {
            get => _customQuantity;
            set
            {
                if (_customQuantity != value)
                {
                    _customQuantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public Menu Menu
        {
            get => _menu;
            set
            {
                if (_menu != value)
                {
                    _menu = value;
                    MenuId = value?.Id ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public Dish Dish
        {
            get => _dish;
            set
            {
                if (_dish != value)
                {
                    _dish = value;
                    DishId = value?.Id ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}