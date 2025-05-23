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
    /// ViewModel for managing categories.
    /// </summary>
    public class CategoryManagementViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly CategoryService _categoryService;
        private readonly DishService _dishService;
        private readonly MenuService _menuService;
        
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Dish> _categoryDishes;
        private ObservableCollection<Menu> _categoryMenus;
        private Category _selectedCategory;
        private Category _editCategory;
        private string _searchTerm;
        private bool _isInEditMode;
        private bool _isAddingNew;
        private bool _isLoadingData;
        
        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }
        
        /// <summary>
        /// Gets or sets the dishes in the selected category.
        /// </summary>
        public ObservableCollection<Dish> CategoryDishes
        {
            get => _categoryDishes;
            set => SetProperty(ref _categoryDishes, value);
        }
        
        /// <summary>
        /// Gets or sets the menus in the selected category.
        /// </summary>
        public ObservableCollection<Menu> CategoryMenus
        {
            get => _categoryMenus;
            set => SetProperty(ref _categoryMenus, value);
        }
        
        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value) && value != null)
                {
                    // If we're not in edit mode, create a copy of the selected category for editing
                    if (!IsInEditMode)
                    {
                        EditCategory = CloneCategory(value);
                        LoadItemsForCategoryAsync(value.CategoryId).ConfigureAwait(false);
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the category being edited.
        /// </summary>
        public Category EditCategory
        {
            get => _editCategory;
            set => SetProperty(ref _editCategory, value);
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
                    FilterCategories();
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
        /// Gets or sets a value indicating whether a new category is being added.
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
        /// Gets the command to add a new category.
        /// </summary>
        public ICommand AddNewCategoryCommand { get; }
        
        /// <summary>
        /// Gets the command to edit a category.
        /// </summary>
        public ICommand EditCategoryCommand { get; }
        
        /// <summary>
        /// Gets the command to delete a category.
        /// </summary>
        public ICommand DeleteCategoryCommand { get; }
        
        /// <summary>
        /// Gets the command to save a category.
        /// </summary>
        public ICommand SaveCategoryCommand { get; }
        
        /// <summary>
        /// Gets the command to cancel editing.
        /// </summary>
        public ICommand CancelEditCommand { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryManagementViewModel"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="categoryService">The category service.</param>
        /// <param name="dishService">The dish service.</param>
        /// <param name="menuService">The menu service.</param>
        public CategoryManagementViewModel(UserService userService, CategoryService categoryService, 
            DishService dishService, MenuService menuService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            
            Title = "Category Management";
            Categories = new ObservableCollection<Category>();
            CategoryDishes = new ObservableCollection<Dish>();
            CategoryMenus = new ObservableCollection<Menu>();
            
            AddNewCategoryCommand = new RelayCommand(AddNewCategory, CanAddNewCategory);
            EditCategoryCommand = new RelayCommand(StartEditingCategory, CanEditCategory);
            DeleteCategoryCommand = new RelayCommand(async () => await DeleteCategoryAsync(), CanDeleteCategory);
            SaveCategoryCommand = new RelayCommand(async () => await SaveCategoryAsync(), CanSaveCategory);
            CancelEditCommand = new RelayCommand(CancelEdit, () => IsInEditMode);
            
            // Load categories
            LoadCategoriesAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Loads all categories.
        /// </summary>
        private async Task LoadCategoriesAsync()
        {
            if (!IsEmployee)
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                var categories = await _categoryService.GetAllCategoriesAsync();
                
                Categories.Clear();
                foreach (var category in categories.OrderBy(c => c.Name))
                {
                    await LoadItemCountForCategoryAsync(category);
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load categories: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Loads the item count for a category.
        /// </summary>
        /// <param name="category">The category.</param>
        private async Task LoadItemCountForCategoryAsync(Category category)
        {
            try
            {
                // Get dishes in this category
                var dishes = await _dishService.GetDishesByCategoryAsync(category.CategoryId);
                
                // Get menus in this category
                var menus = await _menuService.GetMenusByCategoryAsync(category.CategoryId);
                
                // Store the item count
                category.ItemCount = dishes.Count + menus.Count;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load item count: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Loads dishes and menus for the selected category.
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        private async Task LoadItemsForCategoryAsync(int categoryId)
        {
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Get dishes in this category
                var dishes = await _dishService.GetDishesByCategoryAsync(categoryId);
                
                CategoryDishes.Clear();
                foreach (var dish in dishes.OrderBy(d => d.Name))
                {
                    CategoryDishes.Add(dish);
                }
                
                // Get menus in this category
                var menus = await _menuService.GetMenusByCategoryAsync(categoryId);
                
                CategoryMenus.Clear();
                foreach (var menu in menus.OrderBy(m => m.Name))
                {
                    CategoryMenus.Add(menu);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load items for category: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Filters categories based on the search term.
        /// </summary>
        private void FilterCategories()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Load all categories
                LoadCategoriesAsync().ConfigureAwait(false);
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Filter categories by name
                var filteredCategories = Categories.Where(c => 
                    c.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Description != null && c.Description.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
                
                Categories.Clear();
                foreach (var category in filteredCategories.OrderBy(c => c.Name))
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to filter categories: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Determines whether a new category can be added.
        /// </summary>
        /// <returns>True if a new category can be added, false otherwise.</returns>
        private bool CanAddNewCategory()
        {
            return IsEmployee && !IsInEditMode;
        }
        
        /// <summary>
        /// Adds a new category.
        /// </summary>
        private void AddNewCategory()
        {
            // Create a new category
            EditCategory = new Category
            {
                Name = "New Category",
                Description = ""
            };
            
            // Enter edit mode
            IsInEditMode = true;
            IsAddingNew = true;
        }
        
        /// <summary>
        /// Determines whether a category can be edited.
        /// </summary>
        /// <returns>True if a category can be edited, false otherwise.</returns>
        private bool CanEditCategory()
        {
            return IsEmployee && SelectedCategory != null && !IsInEditMode;
        }
        
        /// <summary>
        /// Starts editing a category.
        /// </summary>
        private void StartEditingCategory()
        {
            // Enter edit mode
            IsInEditMode = true;
            IsAddingNew = false;
        }
        
        /// <summary>
        /// Determines whether a category can be deleted.
        /// </summary>
        /// <returns>True if a category can be deleted, false otherwise.</returns>
        private bool CanDeleteCategory()
        {
            return IsEmployee && SelectedCategory != null && !IsInEditMode;
        }
        
        /// <summary>
        /// Deletes a category.
        /// </summary>
        private async Task DeleteCategoryAsync()
        {
            if (!CanDeleteCategory())
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the category '{SelectedCategory.Name}'? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;

                bool success = await _categoryService.DeleteCategoryAsync(SelectedCategory.CategoryId);

                if (success)
                {
                    // Remove from the collection
                    Categories.Remove(SelectedCategory);
                    
                    // Clear selection
                    SelectedCategory = null;
                    EditCategory = null;
                    CategoryDishes.Clear();
                    CategoryMenus.Clear();
                }
                else
                {
                    ErrorMessage = "Failed to delete category. It may contain dishes or menus.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete category: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Determines whether a category can be saved.
        /// </summary>
        /// <returns>True if a category can be saved, false otherwise.</returns>
        private bool CanSaveCategory()
        {
            return IsEmployee && IsInEditMode && EditCategory != null && 
                   !string.IsNullOrWhiteSpace(EditCategory.Name);
        }
        
        /// <summary>
        /// Saves a category.
        /// </summary>
        private async Task SaveCategoryAsync()
        {
            if (!CanSaveCategory())
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;

                Category savedCategory;

                if (IsAddingNew)
                {
                    // Add a new category
                    savedCategory = await _categoryService.AddCategoryAsync(EditCategory);
                    
                    if (savedCategory != null)
                    {
                        // Add to the collection
                        Categories.Add(savedCategory);
                        
                        // Select the new category
                        SelectedCategory = savedCategory;
                    }
                }
                else
                {
                    // Update the existing category
                    bool success = await _categoryService.UpdateCategoryAsync(EditCategory);
                    
                    if (success)
                    {
                        // Update the selected category
                        SelectedCategory.Name = EditCategory.Name;
                        SelectedCategory.Description = EditCategory.Description;
                        
                        // Refresh the list
                        int index = Categories.IndexOf(SelectedCategory);
                        if (index >= 0)
                        {
                            Categories[index] = SelectedCategory;
                        }
                        
                        savedCategory = SelectedCategory;
                    }
                    else
                    {
                        ErrorMessage = "Failed to update category.";
                        return;
                    }
                }

                if (savedCategory != null)
                {
                    // Exit edit mode
                    IsInEditMode = false;
                    IsAddingNew = false;
                }
                else
                {
                    ErrorMessage = "Failed to save category.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to save category: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Cancels editing.
        /// </summary>
        private void CancelEdit()
        {
            // Exit edit mode without saving
            IsInEditMode = false;
            IsAddingNew = false;
            
            // Revert to the selected category
            if (SelectedCategory != null)
            {
                EditCategory = CloneCategory(SelectedCategory);
            }
            else
            {
                EditCategory = null;
            }
        }
        
        /// <summary>
        /// Clones a category.
        /// </summary>
        /// <param name="original">The original category.</param>
        /// <returns>A clone of the original category.</returns>
        private Category CloneCategory(Category original)
        {
            if (original == null)
                return null;

            return new Category
            {
                CategoryId = original.CategoryId,
                Name = original.Name,
                Description = original.Description
            };
        }
    }
}