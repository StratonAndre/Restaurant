using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public class Dish : MenuItem
    {
        private int _dishId;
        private int _portionSize; // in grams
        private decimal _totalQuantity;
        private ObservableCollection<Allergen> _allergens;
        private ObservableCollection<DishImage> _images;
        private ObservableCollection<MenuDish> _menuDishes;

        public override int Id 
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

        public int PortionSize
        {
            get => _portionSize;
            set
            {
                if (_portionSize != value)
                {
                    _portionSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TotalQuantity
        {
            get => _totalQuantity;
            set
            {
                if (_totalQuantity != value)
                {
                    _totalQuantity = value;
                    OnPropertyChanged();
                    // Update IsAvailable property based on quantity
                    IsAvailable = _totalQuantity > 0;
                }
            }
        }

        public ObservableCollection<Allergen> Allergens
        {
            get => _allergens;
            set
            {
                if (_allergens != value)
                {
                    _allergens = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DishImage> Images
        {
            get => _images;
            set
            {
                if (_images != value)
                {
                    _images = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<MenuDish> MenuDishes
        {
            get => _menuDishes;
            set
            {
                if (_menuDishes != value)
                {
                    _menuDishes = value;
                    OnPropertyChanged();
                }
            }
        }

        public Dish()
        {
            Allergens = new ObservableCollection<Allergen>();
            Images = new ObservableCollection<DishImage>();
            MenuDishes = new ObservableCollection<MenuDish>();
            IsAvailable = true;
        }
    }
}