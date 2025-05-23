using RestaurantManager.Models;
using RestaurantManager.Services;
using RestaurantManager.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;

namespace RestaurantManager.ViewModels
{
    public class DishManagementViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly DishService _dishService;
        private readonly CategoryService _categoryService;
        private readonly AllergenService _allergenService;
        private readonly ConfigurationService _configurationService;
        
        private ObservableCollection<Dish> _dishes;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Allergen> _allergens;
        private ObservableCollection<Allergen> _selectedAllergens;
        private ObservableCollection<DishImage> _dishImages;
        private ObservableCollection<Dish> _lowStockDishes;
        
        private Dish _selectedDish;
        private Dish _editDish;
        private Category _selectedCategory;
        private Allergen _selectedAllergen;
        private DishImage _selectedImage;
        
        private bool _isInEditMode;
        private bool _isAddingNew;
        private bool _isLoadingData;
        private decimal _lowStockThreshold;
        private string _selectedImagePath;

        public ObservableCollection<Dish> Dishes
        {
            get => _dishes;
            set => SetProperty(ref _dishes, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Allergen> Allergens
        {
            get => _allergens;
            set => SetProperty(ref _allergens, value);
        }

        public ObservableCollection<Allergen> SelectedAllergens
        {
            get => _selectedAllergens;
            set => SetProperty(ref _selectedAllergens, value);
        }

        public ObservableCollection<DishImage> DishImages
        {
            get => _dishImages;
            set => SetProperty(ref _dishImages, value);
        }

        public ObservableCollection<Dish> LowStockDishes
        {
            get => _lowStockDishes;
            set => SetProperty(ref _lowStockDishes, value);
        }

        public Dish SelectedDish
        {
            get => _selectedDish;
            set
            {
                if (SetProperty(ref _selectedDish, value) && value != null)
                {
                    // If we're not in edit mode, create a copy of the selected dish for editing
                    if (!IsInEditMode)
                    {
                        EditDish = CloneDish(value);
                        
                        // Update selected category
                        SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == value.CategoryId);
                        
                        // Update selected allergens
                        UpdateSelectedAllergens();
                        
                        // Update dish images
                        DishImages = new ObservableCollection<DishImage>(value.Images);
                    }
                }
            }
        }

        public Dish EditDish
        {
            get => _editDish;
            set => SetProperty(ref _editDish, value);
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value) && value != null && EditDish != null)
                {
                    EditDish.CategoryId = value.CategoryId;
                    EditDish.Category = value;
                }
            }
        }

        public Allergen SelectedAllergen
        {
            get => _selectedAllergen;
            set => SetProperty(ref _selectedAllergen, value);
        }

        public DishImage SelectedImage
        {
            get => _selectedImage;
            set => SetProperty(ref _selectedImage, value);
        }

        public bool IsInEditMode
        {
            get => _isInEditMode;
            set => SetProperty(ref _isInEditMode, value);
        }

        public bool IsAddingNew
        {
            get => _isAddingNew;
            set => SetProperty(ref _isAddingNew, value);
        }

        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }

        public decimal LowStockThreshold
        {
            get => _lowStockThreshold;
            set => SetProperty(ref _lowStockThreshold, value);
        }

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set => SetProperty(ref _selectedImagePath, value);
        }

        public User CurrentUser => _userService.CurrentUser;
        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        public bool IsEmployee => _userService.IsEmployee;

        public ICommand RefreshCommand { get; private set; }
        public ICommand AddNewDishCommand { get; private set; }
        public ICommand EditDishCommand { get; private set; }
        public ICommand DeleteDishCommand { get; private set; }
        public ICommand SaveDishCommand { get; private set; }
        public ICommand CancelEditCommand { get; private set; }
        public ICommand AddAllergenCommand { get; private set; }
        public ICommand RemoveAllergenCommand { get; private set; }
        
        // Make BrowseImageCommand have a public setter so it can be set from the code-behind
        public ICommand BrowseImageCommand { get; set; }
        
        public ICommand AddImageCommand { get; private set; }
        public ICommand RemoveImageCommand { get; private set; }

        public DishManagementViewModel(UserService userService, DishService dishService, 
                                     CategoryService categoryService, AllergenService allergenService,
                                     ConfigurationService configurationService)
        {
            _userService = userService;
            _dishService = dishService;
            _categoryService = categoryService;
            _allergenService = allergenService;
            _configurationService = configurationService;
            
            Title = "Dish Management";
            Dishes = new ObservableCollection<Dish>();
            Categories = new ObservableCollection<Category>();
            Allergens = new ObservableCollection<Allergen>();
            SelectedAllergens = new ObservableCollection<Allergen>();
            DishImages = new ObservableCollection<DishImage>();
            LowStockDishes = new ObservableCollection<Dish>();

            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            AddNewDishCommand = new RelayCommand(AddNewDish, CanAddNewDish);
            EditDishCommand = new RelayCommand(StartEditingDish, CanEditDish);
            DeleteDishCommand = new RelayCommand(async () => await DeleteDishAsync(), CanDeleteDish);
            SaveDishCommand = new RelayCommand(async () => await SaveDishAsync(), CanSaveDish);
            CancelEditCommand = new RelayCommand(CancelEdit, () => IsInEditMode);
            AddAllergenCommand = new RelayCommand(AddAllergen, CanAddAllergen);
            RemoveAllergenCommand = new RelayCommand<Allergen>(RemoveAllergen);
            
            // Initialize BrowseImageCommand with a default implementation
            BrowseImageCommand = new RelayCommand(() => { /* Will be overridden in code-behind */ }, () => IsInEditMode);
            
            AddImageCommand = new RelayCommand(async () => await AddImageAsync(), CanAddImage);
            RemoveImageCommand = new RelayCommand(async () => await RemoveImageAsync(), CanRemoveImage);
            
            // Initial data load
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            if (!IsEmployee)
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Load dishes
                var dishes = await _dishService.GetAllDishesAsync();
                Dishes = new ObservableCollection<Dish>(dishes.OrderBy(d => d.Name));
                
                // Load categories
                var categories = await _categoryService.GetAllCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories.OrderBy(c => c.Name));
                
                // Load allergens
                var allergens = await _allergenService.GetAllAllergensAsync();
                Allergens = new ObservableCollection<Allergen>(allergens.OrderBy(a => a.Name));
                
                // Get low stock threshold
                LowStockThreshold = _configurationService.LowStockThreshold;
                
                // Load low stock dishes
                var lowStockDishes = await _dishService.GetLowStockDishesAsync(LowStockThreshold);
                LowStockDishes = new ObservableCollection<Dish>(lowStockDishes.OrderBy(d => d.TotalQuantity));
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

        private bool CanAddNewDish()
        {
            return IsEmployee && !IsInEditMode;
        }

        private void AddNewDish()
        {
            // Create a new dish
            EditDish = new Dish
            {
                Name = "New Dish",
                Description = "",
                PortionSize = 100,
                Price = 0,
                TotalQuantity = 0,
                IsAvailable = true
            };
            
            // Clear selected allergens and images
            SelectedAllergens.Clear();
            DishImages.Clear();
            
            // Set default category if available
            if (Categories.Count > 0)
            {
                SelectedCategory = Categories[0];
                EditDish.CategoryId = SelectedCategory.CategoryId;
                EditDish.Category = SelectedCategory;
            }
            
            // Enter edit mode
            IsInEditMode = true;
            IsAddingNew = true;
        }

        private bool CanEditDish()
        {
            return IsEmployee && SelectedDish != null && !IsInEditMode;
        }

        private void StartEditingDish()
        {
            // Enter edit mode
            IsInEditMode = true;
            IsAddingNew = false;
        }

        private bool CanDeleteDish()
        {
            return IsEmployee && SelectedDish != null && !IsInEditMode;
        }

        private async Task DeleteDishAsync()
        {
            if (!CanDeleteDish())
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                bool confirmed = true; // In a real app, show a confirmation dialog
                
                if (confirmed)
                {
                    // Delete the dish
                    bool success = await _dishService.DeleteDishAsync(SelectedDish.Id);
                    
                    if (success)
                    {
                        // Refresh the dishes
                        await LoadDataAsync();
                        
                        // Clear selection
                        SelectedDish = null;
                        EditDish = null;
                    }
                    else
                    {
                        ErrorMessage = "Failed to delete dish. It may be referenced by menus or orders.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete dish: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private bool CanSaveDish()
        {
            return IsEmployee && IsInEditMode && EditDish != null && 
                   !string.IsNullOrWhiteSpace(EditDish.Name) && 
                   EditDish.PortionSize > 0 && 
                   EditDish.Price >= 0 && 
                   EditDish.TotalQuantity >= 0 && 
                   EditDish.CategoryId > 0;
        }

        private async Task SaveDishAsync()
        {
            if (!CanSaveDish())
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                Dish savedDish;
                
                if (IsAddingNew)
                {
                    // Add a new dish
                    savedDish = await _dishService.AddDishAsync(EditDish);
                    
                    if (savedDish != null)
                    {
                        // Add selected allergens
                        foreach (var allergen in SelectedAllergens)
                        {
                            await _allergenService.AddAllergenToDishAsync(savedDish.Id, allergen.AllergenId);
                        }
                    }
                }
                else
                {
                    // Update the existing dish
                    bool success = await _dishService.UpdateDishAsync(EditDish);
                    
                    if (success)
                    {
                        savedDish = EditDish;
                        
                        // Update allergens
                        // First, get current allergens
                        var currentAllergens = await _allergenService.GetAllergensForDishAsync(savedDish.Id);
                        
                        // Remove allergens that are no longer selected
                        foreach (var allergen in currentAllergens)
                        {
                            if (!SelectedAllergens.Any(a => a.AllergenId == allergen.AllergenId))
                            {
                                await _allergenService.RemoveAllergenFromDishAsync(savedDish.Id, allergen.AllergenId);
                            }
                        }
                        
                        // Add newly selected allergens
                        foreach (var allergen in SelectedAllergens)
                        {
                            if (!currentAllergens.Any(a => a.AllergenId == allergen.AllergenId))
                            {
                                await _allergenService.AddAllergenToDishAsync(savedDish.Id, allergen.AllergenId);
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = "Failed to update dish.";
                        return;
                    }
                }
                
                if (savedDish != null)
                {
                    // Refresh the dishes
                    await LoadDataAsync();
                    
                    // Select the saved dish
                    SelectedDish = Dishes.FirstOrDefault(d => d.Id == savedDish.Id);
                    
                    // Exit edit mode
                    IsInEditMode = false;
                    IsAddingNew = false;
                }
                else
                {
                    ErrorMessage = "Failed to save dish.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to save dish: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private void CancelEdit()
        {
            // Exit edit mode without saving
            IsInEditMode = false;
            IsAddingNew = false;
            
            // Revert to the selected dish
            if (SelectedDish != null)
            {
                EditDish = CloneDish(SelectedDish);
                UpdateSelectedAllergens();
            }
            else
            {
                EditDish = null;
                SelectedAllergens.Clear();
            }
        }

        private bool CanAddAllergen()
        {
            return IsEmployee && IsInEditMode && SelectedAllergen != null && 
                   !SelectedAllergens.Any(a => a.AllergenId == SelectedAllergen.AllergenId);
        }

        private void AddAllergen()
        {
            if (!CanAddAllergen())
                return;

            // Add the selected allergen to the list
            SelectedAllergens.Add(SelectedAllergen);
            
            // Clear the selection
            SelectedAllergen = null;
        }

        private void RemoveAllergen(Allergen allergen)
        {
            if (IsInEditMode && allergen != null)
            {
                // Remove the allergen from the list
                SelectedAllergens.Remove(allergen);
            }
        }

        private bool CanAddImage()
        {
            return IsEmployee && IsInEditMode && !string.IsNullOrWhiteSpace(SelectedImagePath);
        }

        private async Task AddImageAsync()
        {
            if (!CanAddImage())
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                if (IsAddingNew)
                {
                    // For a new dish, just add to the local collection
                    // The images will be added to the database when the dish is saved
                    string fileName = Path.GetFileName(SelectedImagePath);
                    string targetPath = Path.Combine("Resources", "Images", fileName);
                    
                    DishImages.Add(new DishImage
                    {
                        ImagePath = targetPath
                    });
                }
                else
                {
                    // For an existing dish, add to the database
                    // In a real app, copy the image to the appropriate folder
                    string fileName = Path.GetFileName(SelectedImagePath);
                    string targetPath = Path.Combine("Resources", "Images", fileName);
                    
                    // Ensure the target directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    
                    // Copy the file
                    File.Copy(SelectedImagePath, targetPath, true);
                    
                    // Add the image to the database
                    var image = new DishImage
                    {
                        DishId = SelectedDish.Id,
                        ImagePath = targetPath
                    };
                    
                    var savedImage = await _dishService.AddDishImageAsync(image);
                    
                    if (savedImage != null)
                    {
                        // Add to the local collection
                        DishImages.Add(savedImage);
                    }
                    else
                    {
                        ErrorMessage = "Failed to add image.";
                    }
                }
                
                // Clear the selected image path
                SelectedImagePath = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to add image: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private bool CanRemoveImage()
        {
            return IsEmployee && IsInEditMode && SelectedImage != null;
        }

        private async Task RemoveImageAsync()
        {
            if (!CanRemoveImage())
                return;

            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                if (IsAddingNew || SelectedImage.ImageId == 0)
                {
                    // For a new dish or a local image, just remove from the local collection
                    DishImages.Remove(SelectedImage);
                }
                else
                {
                    // For an existing image, remove from the database
                    bool success = await _dishService.DeleteDishImageAsync(SelectedImage.ImageId);
                    
                    if (success)
                    {
                        // Remove from the local collection
                        DishImages.Remove(SelectedImage);
                    }
                    else
                    {
                        ErrorMessage = "Failed to remove image.";
                    }
                }
                
                // Clear the selection
                SelectedImage = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to remove image: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        // Helper method to clone a dish
        private Dish CloneDish(Dish original)
        {
            if (original == null)
                return null;

            return new Dish
            {
                Id = original.Id,
                Name = original.Name,
                Description = original.Description,
                CategoryId = original.CategoryId,
                Category = original.Category,
                PortionSize = original.PortionSize,
                Price = original.Price,
                TotalQuantity = original.TotalQuantity,
                IsAvailable = original.IsAvailable
            };
        }

        // Helper method to update selected allergens based on the selected dish
        private void UpdateSelectedAllergens()
        {
            SelectedAllergens.Clear();
            
            if (SelectedDish != null && SelectedDish.Allergens != null)
            {
                foreach (var allergen in SelectedDish.Allergens)
                {
                    SelectedAllergens.Add(allergen);
                }
            }
        }
    }
}