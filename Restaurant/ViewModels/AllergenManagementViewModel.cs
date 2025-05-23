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
    /// ViewModel for managing allergens.
    /// </summary>
    public class AllergenManagementViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly AllergenService _allergenService;
        private readonly DishService _dishService;
        
        private ObservableCollection<Allergen> _allergens;
        private ObservableCollection<Dish> _allergenDishes;
        private Allergen _selectedAllergen;
        private Allergen _editAllergen;
        private string _searchTerm;
        private bool _isInEditMode;
        private bool _isAddingNew;
        private bool _isLoadingData;
        
        /// <summary>
        /// Gets or sets the allergens.
        /// </summary>
        public ObservableCollection<Allergen> Allergens
        {
            get => _allergens;
            set => SetProperty(ref _allergens, value);
        }
        
        /// <summary>
        /// Gets or sets the dishes containing the selected allergen.
        /// </summary>
        public ObservableCollection<Dish> AllergenDishes
        {
            get => _allergenDishes;
            set => SetProperty(ref _allergenDishes, value);
        }
        
        /// <summary>
        /// Gets or sets the selected allergen.
        /// </summary>
        public Allergen SelectedAllergen
        {
            get => _selectedAllergen;
            set
            {
                if (SetProperty(ref _selectedAllergen, value) && value != null)
                {
                    // If we're not in edit mode, create a copy of the selected allergen for editing
                    if (!IsInEditMode)
                    {
                        EditAllergen = CloneAllergen(value);
                        LoadDishesForAllergenAsync(value.AllergenId).ConfigureAwait(false);
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the allergen being edited.
        /// </summary>
        public Allergen EditAllergen
        {
            get => _editAllergen;
            set => SetProperty(ref _editAllergen, value);
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
                    FilterAllergens();
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
        /// Gets or sets a value indicating whether a new allergen is being added.
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
        /// Gets the command to add a new allergen.
        /// </summary>
        public ICommand AddNewAllergenCommand { get; }
        
        /// <summary>
        /// Gets the command to edit an allergen.
        /// </summary>
        public ICommand EditAllergenCommand { get; }
        
        /// <summary>
        /// Gets the command to delete an allergen.
        /// </summary>
        public ICommand DeleteAllergenCommand { get; }
        
        /// <summary>
        /// Gets the command to save an allergen.
        /// </summary>
        public ICommand SaveAllergenCommand { get; }
        
        /// <summary>
        /// Gets the command to cancel editing.
        /// </summary>
        public ICommand CancelEditCommand { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AllergenManagementViewModel"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="allergenService">The allergen service.</param>
        /// <param name="dishService">The dish service.</param>
        public AllergenManagementViewModel(UserService userService, AllergenService allergenService, DishService dishService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _allergenService = allergenService ?? throw new ArgumentNullException(nameof(allergenService));
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
            
            Title = "Allergen Management";
            Allergens = new ObservableCollection<Allergen>();
            AllergenDishes = new ObservableCollection<Dish>();
            
            AddNewAllergenCommand = new RelayCommand(AddNewAllergen, CanAddNewAllergen);
            EditAllergenCommand = new RelayCommand(StartEditingAllergen, CanEditAllergen);
            DeleteAllergenCommand = new RelayCommand(DeleteAllergen, CanDeleteAllergen);
            SaveAllergenCommand = new RelayCommand(SaveAllergen, CanSaveAllergen);
            CancelEditCommand = new RelayCommand(CancelEdit, () => IsInEditMode);
            
            // Load allergens
            LoadAllergensAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Loads all allergens.
        /// </summary>
        private async Task LoadAllergensAsync()
        {
            if (!IsEmployee)
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                var allergens = await _allergenService.GetAllAllergensAsync();
                
                Allergens.Clear();
                foreach (var allergen in allergens.OrderBy(a => a.Name))
                {
                    Allergens.Add(allergen);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load allergens: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Loads dishes containing the selected allergen.
        /// </summary>
        /// <param name="allergenId">The allergen ID.</param>
        private async Task LoadDishesForAllergenAsync(int allergenId)
        {
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                var dishes = await _dishService.GetDishesByAllergenAsync(allergenId);
                
                AllergenDishes.Clear();
                foreach (var dish in dishes.OrderBy(d => d.Name))
                {
                    AllergenDishes.Add(dish);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load dishes for allergen: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Filters allergens based on the search term.
        /// </summary>
        private void FilterAllergens()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Load all allergens
                LoadAllergensAsync().ConfigureAwait(false);
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Filter allergens by name
                var filteredAllergens = Allergens.Where(a => 
                    a.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (a.Description != null && a.Description.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
                
                Allergens.Clear();
                foreach (var allergen in filteredAllergens.OrderBy(a => a.Name))
                {
                    Allergens.Add(allergen);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to filter allergens: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Determines whether a new allergen can be added.
        /// </summary>
        /// <returns>True if a new allergen can be added, false otherwise.</returns>
        private bool CanAddNewAllergen()
        {
            return IsEmployee && !IsInEditMode;
        }
        
        /// <summary>
        /// Adds a new allergen.
        /// </summary>
        private void AddNewAllergen()
        {
            // Create a new allergen
            EditAllergen = new Allergen
            {
                Name = "New Allergen",
                Description = ""
            };
            
            // Enter edit mode
            IsInEditMode = true;
            IsAddingNew = true;
        }
        
        /// <summary>
        /// Determines whether an allergen can be edited.
        /// </summary>
        /// <returns>True if an allergen can be edited, false otherwise.</returns>
        private bool CanEditAllergen()
        {
            return IsEmployee && SelectedAllergen != null && !IsInEditMode;
        }
        
        /// <summary>
        /// Starts editing an allergen.
        /// </summary>
        private void StartEditingAllergen()
        {
            // Enter edit mode
            IsInEditMode = true;
            IsAddingNew = false;
        }
        
        /// <summary>
        /// Determines whether an allergen can be deleted.
        /// </summary>
        /// <returns>True if an allergen can be deleted, false otherwise.</returns>
        private bool CanDeleteAllergen()
        {
            return IsEmployee && SelectedAllergen != null && !IsInEditMode;
        }
        
        /// <summary>
        /// Deletes an allergen.
        /// </summary>
        private async void DeleteAllergen()
        {
            if (!CanDeleteAllergen())
            {
                return;
            }
            
            var result = MessageBox.Show(
                $"Are you sure you want to delete the allergen '{SelectedAllergen.Name}'? " +
                "This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
                
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                 
                bool success = await _allergenService.DeleteAllergenAsync(SelectedAllergen.AllergenId);
                
                if (success)
                {
                    // Remove from the collection
                    Allergens.Remove(SelectedAllergen);
                    
                    // Clear selection
                    SelectedAllergen = null;
                    EditAllergen = null;
                    AllergenDishes.Clear();
                }
                else
                {
                    ErrorMessage = "Failed to delete allergen. It may be referenced by dishes.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete allergen: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Determines whether an allergen can be saved.
        /// </summary>
        /// <returns>True if an allergen can be saved, false otherwise.</returns>
        private bool CanSaveAllergen()
        {
            return IsEmployee && IsInEditMode && EditAllergen != null && 
                   !string.IsNullOrWhiteSpace(EditAllergen.Name);
        }
        
        /// <summary>
        /// Saves an allergen.
        /// </summary>
        private async void SaveAllergen()
        {
            if (!CanSaveAllergen())
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                Allergen savedAllergen;
                
                if (IsAddingNew)
                {
                    // Add a new allergen
                    savedAllergen = await _allergenService.AddAllergenAsync(EditAllergen);
                    
                    if (savedAllergen != null)
                    {
                        // Add to the collection
                        Allergens.Add(savedAllergen);
                        
                        // Select the new allergen
                        SelectedAllergen = savedAllergen;
                    }
                    else
                    {
                        ErrorMessage = "Failed to add allergen.";
                        return;
                    }
                }
                else
                {
                    // Update the existing allergen
                    bool success = await _allergenService.UpdateAllergenAsync(EditAllergen);
                    
                    if (success)
                    {
                        // Update the selected allergen
                        SelectedAllergen.Name = EditAllergen.Name;
                        SelectedAllergen.Description = EditAllergen.Description;
                        
                        // Refresh the list
                        int index = Allergens.IndexOf(SelectedAllergen);
                        if (index >= 0)
                        {
                            Allergens[index] = SelectedAllergen;
                        }
                    }
                    else
                    {
                        ErrorMessage = "Failed to update allergen.";
                        return;
                    }
                }
                
                // Exit edit mode
                IsInEditMode = false;
                IsAddingNew = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to save allergen: {ex.Message}";
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
            
            // Revert to the selected allergen
            if (SelectedAllergen != null)
            {
                EditAllergen = CloneAllergen(SelectedAllergen);
            }
            else
            {
                EditAllergen = null;
            }
        }
        
        /// <summary>
        /// Clones an allergen.
        /// </summary>
        /// <param name="original">The original allergen.</param>
        /// <returns>A clone of the original allergen.</returns>
        private Allergen CloneAllergen(Allergen original)
        {
            if (original == null)
            {
                return null;
            }
            
            return new Allergen
            {
                AllergenId = original.AllergenId,
                Name = original.Name,
                Description = original.Description
            };
        }
    }
}