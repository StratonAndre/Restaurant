using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    /// <summary>
    /// Represents a join entity between a Dish and an Allergen.
    /// </summary>
    public class DishAllergen : INotifyPropertyChanged
    {
        private int _dishAllergenId;
        private int _dishId;
        private int _allergenId;
        private Dish _dish;
        private Allergen _allergen;

        /// <summary>
        /// Gets or sets the dish allergen ID.
        /// </summary>
        public int DishAllergenId
        {
            get => _dishAllergenId;
            set
            {
                if (_dishAllergenId != value)
                {
                    _dishAllergenId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the dish ID.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the allergen ID.
        /// </summary>
        public int AllergenId
        {
            get => _allergenId;
            set
            {
                if (_allergenId != value)
                {
                    _allergenId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the associated dish.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the associated allergen.
        /// </summary>
        public Allergen Allergen
        {
            get => _allergen;
            set
            {
                if (_allergen != value)
                {
                    _allergen = value;
                    AllergenId = value?.AllergenId ?? 0;
                    OnPropertyChanged();
                }
            }
        }

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