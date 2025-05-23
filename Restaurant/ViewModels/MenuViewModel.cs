using RestaurantManager.Models;
using RestaurantManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RestaurantManager.Utilities;
namespace RestaurantManager.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private readonly DishService _dishService;
        private readonly MenuService _menuService;
        private readonly UserService _userService;

        private ObservableCollection<Category> _categories;
        private ObservableCollection<MenuItem> _menuItems;
        private Category _selectedCategory;
        private MenuItem _selectedMenuItem;
        private bool _isLoadingData;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<MenuItem> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    LoadMenuItemsForCategoryAsync();
                }
            }
        }

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetProperty(ref _selectedMenuItem, value);
        }

        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }

        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        public bool IsClient => _userService.IsClient;
        public User CurrentUser => _userService.CurrentUser;

        public ICommand RefreshCommand { get; private set; }
        public ICommand AddToCartCommand { get; private set; }
        
        public MenuViewModel(CategoryService categoryService, DishService dishService, 
                            MenuService menuService, UserService userService)
        {
            _categoryService = categoryService;
            _dishService = dishService;
            _menuService = menuService;
            _userService = userService;

            Title = "Restaurant Menu";
            Categories = new ObservableCollection<Category>();
            MenuItems = new ObservableCollection<MenuItem>();

            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            AddToCartCommand = new RelayCommand<MenuItem>(AddToCart, CanAddToCart);
            
            // Initial data load
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoadingData = true;
                
                // Load categories
                var categories = await _categoryService.GetAllCategoriesAsync();
                Categories.Clear();
                
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
                
                // If no category is selected, select the first one
                if (SelectedCategory == null && Categories.Count > 0)
                {
                    SelectedCategory = Categories[0];
                }
                else if (SelectedCategory != null)
                {
                    // Refresh the selected category's items
                    await LoadMenuItemsForCategoryAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load menu data: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private async Task LoadMenuItemsForCategoryAsync()
        {
            if (SelectedCategory == null)
                return;

            try
            {
                IsLoadingData = true;
                
                // Load dishes for the selected category
                var dishes = await _dishService.GetDishesByCategoryAsync(SelectedCategory.CategoryId);
                
                // Load menus for the selected category
                var menus = await _menuService.GetMenusByCategoryAsync(SelectedCategory.CategoryId);
                
                // Combine dishes and menus
                MenuItems = new ObservableCollection<MenuItem>(
                    dishes.Cast<MenuItem>().Concat(menus.Cast<MenuItem>())
                    .OrderBy(item => item.Name)
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load menu items: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private bool CanAddToCart(MenuItem menuItem)
        {
            return IsClient && menuItem != null && menuItem.IsAvailable;
        }

        private void AddToCart(MenuItem menuItem)
        {
            // This will be implemented in the ShoppingCartViewModel
            // For now, we'll just provide a placeholder implementation
            if (menuItem != null)
            {
                // Call to a service that manages the shopping cart
                // ShoppingCartService.AddItem(menuItem, 1);
                
                ErrorMessage = $"{menuItem.Name} added to your cart.";
            }
        }
    }
}