using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public class Allergen : INotifyPropertyChanged
    {
        private int _allergenId;
        private string _name;
        private string _description;
        private ObservableCollection<Dish> _dishes;

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

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Dish> Dishes
        {
            get => _dishes;
            set
            {
                if (_dishes != value)
                {
                    _dishes = value;
                    OnPropertyChanged();
                }
            }
        }

        public Allergen()
        {
            Dishes = new ObservableCollection<Dish>();
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