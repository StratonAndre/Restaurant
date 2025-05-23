using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public class DishImage : INotifyPropertyChanged
    {
        private int _imageId;
        private int _dishId;
        private string _imagePath;
        private Dish _dish;

        public int ImageId
        {
            get => _imageId;
            set
            {
                if (_imageId != value)
                {
                    _imageId = value;
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

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
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