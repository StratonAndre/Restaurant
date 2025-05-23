using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public class Category : INotifyPropertyChanged
    {
        private int _categoryId;
        private string _name;
        private string _description;
        private int _itemCount;
        private ObservableCollection<Dish> _dishes;
        private ObservableCollection<Menu> _menus;

        public int CategoryId
        {
            get => _categoryId;
            set
            {
                if (_categoryId != value)
                {
                    _categoryId = value;
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

        public int ItemCount
        {
            get => _itemCount;
            set
            {
                if (_itemCount != value)
                {
                    _itemCount = value;
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
                    UpdateItemCount();
                }
            }
        }

        public ObservableCollection<Menu> Menus
        {
            get => _menus;
            set
            {
                if (_menus != value)
                {
                    _menus = value;
                    OnPropertyChanged();
                    UpdateItemCount();
                }
            }
        }

        public Category()
        {
            Dishes = new ObservableCollection<Dish>();
            Menus = new ObservableCollection<Menu>();
        }

        private void UpdateItemCount()
        {
            ItemCount = (Dishes?.Count ?? 0) + (Menus?.Count ?? 0);
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