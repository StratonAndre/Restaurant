using RestaurantManager.Models;
using RestaurantManager.Services;
using RestaurantManager.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestaurantManager.ViewModels
{
    /// <summary>
    /// ViewModel for managing menus.
    /// </summary>
    public class MenuManagementViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly MenuService _menuService;
        private readonly CategoryService _categoryService;
        private readonly DishService _dishService;
        private readonly ConfigurationService _configurationService;

        private ObservableCollection<Menu> _menus;
        private ObservableCollection<Menu> _allMenus; // Store all menus for filtering
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Dish> _availableDishes;
        private ObservableCollection<MenuDish> _menuDishes;

        private Menu _selectedMenu;
        private Menu _editMenu;
        private Category _selectedCategory;
        private Dish _selectedDish;
        private int _customQuantity;
        private string _searchTerm;

        private bool _isInEditMode;
        private bool _isAddingNew;
        private bool _isLoadingData;

        /// <summary>
        /// Gets or sets the menus.
        /// </summary>
        public ObservableCollection<Menu> Menus
        {
            get => _menus;
            set => SetProperty(ref _menus, value);
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        /// <summary>
        /// Gets or sets the available dishes.
        /// </summary>
        public ObservableCollection<Dish> AvailableDishes
        {
            get => _availableDishes;
            set => SetProperty(ref _availableDishes, value);
        }

        /// <summary>
        /// Gets or sets the menu dishes.
        /// </summary>
        public ObservableCollection<MenuDish> MenuDishes
        {
            get => _menuDishes;
            set => SetProperty(ref _menuDishes, value);
        }

        /// <summary>
        /// Gets or sets the selected menu.
        /// </summary>
        public Menu SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                if (SetProperty(ref _selectedMenu, value) && value != null)
                {
                    // If we're not in edit mode, create a copy of the selected menu for editing
                    if (!IsInEditMode)
                    {
                        EditMenu = CloneMenu(value);

                        // Update selected category
                        SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == value.CategoryId);

                        // Update menu dishes
                        MenuDishes = new ObservableCollection<MenuDish>(value.MenuDishes);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the menu being edited.
        /// </summary>
        public Menu EditMenu
        {
            get => _editMenu;
            set => SetProperty(ref _editMenu, value);
        }

        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value) && value != null && EditMenu != null)
                {
                    EditMenu.CategoryId = value.CategoryId;
                    EditMenu.Category = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected dish.
        /// </summary>
        public Dish SelectedDish
        {
            get => _selectedDish;
            set => SetProperty(ref _selectedDish, value);
        }

        /// <summary>
        /// Gets or sets the custom quantity for the selected dish.
        /// </summary>
        public int CustomQuantity
        {
            get => _customQuantity;
            set => SetProperty(ref _customQuantity, value);
        }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilterMenus();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set => SetProperty(ref _isInEditMode, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether a new menu is being added.
        /// </summary>
        public bool IsAddingNew
        {
            get => _isAddingNew;
            set => SetProperty(ref _isAddingNew, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether data is being loaded.
        /// </summary>
        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        public User CurrentUser => _userService.CurrentUser;

        /// <summary>
        /// Gets a value indicating whether the user is authenticated.
        /// </summary>
        public bool IsUserAuthenticated => _userService.IsAuthenticated;

        /// <summary>
        /// Gets a value indicating whether the user is an employee.
        /// </summary>
        public bool IsEmployee => _userService.IsEmployee;

        /// <summary>
        /// Gets the command to add a new menu.
        /// </summary>
        public ICommand AddNewMenuCommand { get; }

        /// <summary>
        /// Gets the command to edit a menu.
        /// </summary>
        public ICommand EditMenuCommand { get; }

        /// <summary>
        /// Gets the command to delete a menu.
        /// </summary>
        public ICommand DeleteMenuCommand { get; }

        /// <summary>
        /// Gets the command to save a menu.
        /// </summary>
        public ICommand SaveMenuCommand { get; }

        /// <summary>
        /// Gets the command to cancel editing.
        /// </summary>
        public ICommand CancelEditCommand { get; }

        /// <summary>
        /// Gets the command to add a dish to the menu.
        /// </summary>
        public ICommand AddDishToMenuCommand { get; }

        /// <summary>
        /// Gets the command to remove a dish from the menu.
        /// </summary>
        public ICommand RemoveDishFromMenuCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuManagementViewModel"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="menuService">The menu service.</param>
        /// <param name="categoryService">The category service.</param>
        /// <param name="dishService">The dish service.</param>
        /// <param name="configurationService">The configuration service.</param>
        public MenuManagementViewModel(UserService userService, MenuService menuService,
            CategoryService categoryService, DishService dishService, ConfigurationService configurationService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
            _configurationService =
                configurationService ?? throw new ArgumentNullException(nameof(configurationService));

            Title = "Menu Management";
            Menus = new ObservableCollection<Menu>();
            _allMenus = new ObservableCollection<Menu>();
            Categories = new ObservableCollection<Category>();
            AvailableDishes = new ObservableCollection<Dish>();
            MenuDishes = new ObservableCollection<MenuDish>();

            // Initialize commands - Note: Changed method name to avoid conflict
            AddNewMenuCommand = new RelayCommand(AddNewMenu, CanAddNewMenu);
            EditMenuCommand = new RelayCommand(StartEditingMenu, CanEditMenu);
            DeleteMenuCommand = new RelayCommand(async () => await DeleteMenuAsync(), CanDeleteMenu);
            SaveMenuCommand = new RelayCommand(async () => await SaveMenuAsync(), CanSaveMenu);
            CancelEditCommand = new RelayCommand(CancelEdit, () => IsInEditMode);
            AddDishToMenuCommand = new RelayCommand(AddDishToMenu, CanAddDishToMenu);
            RemoveDishFromMenuCommand = new RelayCommand<MenuDish>(RemoveDishFromMenu);

            // Load initial data
            LoadDataAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Loads all data needed for menu management.
        /// </summary>
        private async Task LoadDataAsync()
        {
            if (!IsEmployee)
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;

                // Load menus
                var menus = await _menuService.GetAllMenusAsync();
                _allMenus = new ObservableCollection<Menu>(menus.OrderBy(m => m.Name));
                Menus = new ObservableCollection<Menu>(_allMenus);

                // Load categories
                var categories = await _categoryService.GetAllCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories.OrderBy(c => c.Name));

                // Load available dishes
                var dishes = await _dishService.GetAllDishesAsync();
                AvailableDishes = new ObservableCollection<Dish>(dishes.Where(d => d.IsAvailable).OrderBy(d => d.Name));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        /// <summary>
        /// Filters menus based on search term.
        /// </summary>
        private void FilterMenus()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                Menus = new ObservableCollection<Menu>(_allMenus);
            }
            else
            {
                var filteredMenus = _allMenus.Where(m => 
                    m.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.Description != null && m.Description.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
                
                Menus = new ObservableCollection<Menu>(filteredMenus);
            }
        }

        /// <summary>
        /// Creates a clone of a menu for editing.
        /// </summary>
        /// <param name="original">The original menu to clone.</param>
        /// <returns>A cloned menu.</returns>
        private Menu CloneMenu(Menu original)
        {
            if (original == null)
                return null;

            return new Menu
            {
                Id = original.Id,
                Name = original.Name,
                Description = original.Description,
                CategoryId = original.CategoryId,
                Category = original.Category,
                DiscountPercentage = original.DiscountPercentage,
                IsAvailable = original.IsAvailable
            };
        }

        private bool CanAddNewMenu()
        {
            return IsEmployee && !IsInEditMode;
        }

        private void AddNewMenu()
        {
            EditMenu = new Menu
            {
                Name = "New Menu",
                Description = "",
                DiscountPercentage = _configurationService.MenuDiscountPercentage,
                IsAvailable = true
            };

            MenuDishes.Clear();

            // Set default category if available
            if (Categories.Count > 0)
            {
                SelectedCategory = Categories[0];
                EditMenu.CategoryId = SelectedCategory.CategoryId;
                EditMenu.Category = SelectedCategory;
            }

            IsInEditMode = true;
            IsAddingNew = true;
        }

        private bool CanEditMenu()
        {
            return IsEmployee && SelectedMenu != null && !IsInEditMode;
        }

        // Renamed method to avoid conflict with EditMenu property
        private void StartEditingMenu()
        {
            IsInEditMode = true;
            IsAddingNew = false;
        }

        private bool CanDeleteMenu()
        {
            return IsEmployee && SelectedMenu != null && !IsInEditMode;
        }

        private async Task DeleteMenuAsync()
        {
            if (!CanDeleteMenu())
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the menu '{SelectedMenu.Name}'? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;

                bool success = await _menuService.DeleteMenuAsync(SelectedMenu.Id);

                if (success)
                {
                    await LoadDataAsync();
                    SelectedMenu = null;
                    EditMenu = null;
                    MenuDishes.Clear();
                }
                else
                {
                    ErrorMessage = "Failed to delete menu. It may be referenced by orders.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete menu: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private bool CanSaveMenu()
        {
            return IsEmployee && IsInEditMode && EditMenu != null && 
                   !string.IsNullOrWhiteSpace(EditMenu.Name) && 
                   EditMenu.CategoryId > 0 && 
                   MenuDishes.Count > 0;
        }

        private async Task SaveMenuAsync()
        {
            if (!CanSaveMenu())
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;

                Menu savedMenu;

                if (IsAddingNew)
                {
                    // Set menu dishes for the new menu
                    EditMenu.MenuDishes = MenuDishes;
                    savedMenu = await _menuService.AddMenuAsync(EditMenu);
                }
                else
                {
                    bool success = await _menuService.UpdateMenuAsync(EditMenu);
                    if (success)
                    {
                        savedMenu = EditMenu;
                        // TODO: Update menu dishes if needed
                    }
                    else
                    {
                        ErrorMessage = "Failed to update menu.";
                        return;
                    }
                }

                if (savedMenu != null)
                {
                    await LoadDataAsync();
                    SelectedMenu = Menus.FirstOrDefault(m => m.Id == savedMenu.Id);
                    IsInEditMode = false;
                    IsAddingNew = false;
                }
                else
                {
                    ErrorMessage = "Failed to save menu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to save menu: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private void CancelEdit()
        {
            IsInEditMode = false;
            IsAddingNew = false;

            if (SelectedMenu != null)
            {
                EditMenu = CloneMenu(SelectedMenu);
                MenuDishes = new ObservableCollection<MenuDish>(SelectedMenu.MenuDishes);
            }
            else
            {
                EditMenu = null;
                MenuDishes.Clear();
            }
        }

        private bool CanAddDishToMenu()
        {
            return IsEmployee && IsInEditMode && SelectedDish != null && CustomQuantity > 0 &&
                   !MenuDishes.Any(md => md.DishId == SelectedDish.Id);
        }

        private void AddDishToMenu()
        {
            if (!CanAddDishToMenu())
                return;

            var menuDish = new MenuDish
            {
                DishId = SelectedDish.Id,
                Dish = SelectedDish,
                CustomQuantity = CustomQuantity
            };

            MenuDishes.Add(menuDish);

            // Clear selections
            SelectedDish = null;
            CustomQuantity = 0;
        }

        private void RemoveDishFromMenu(MenuDish menuDish)
        {
            if (IsInEditMode && menuDish != null)
            {
                MenuDishes.Remove(menuDish);
            }
        }
    }
}