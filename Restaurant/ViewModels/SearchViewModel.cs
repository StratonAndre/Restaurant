using RestaurantManager.Models;
using RestaurantManager.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RestaurantManager.Utilities;
namespace RestaurantManager.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly SearchService _searchService;
        private readonly AllergenService _allergenService;
        private readonly UserService _userService;

        private string _searchKeyword;
        private ObservableCollection<Allergen> _allergens;
        private Allergen _selectedAllergen;
        private bool _excludeAllergen;
        private ObservableCollection<Dish> _dishResults;
        private ObservableCollection<Menu> _menuResults;
        private MenuItem _selectedMenuItem;
        private bool _isSearching;
        private int _totalResults;

        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetProperty(ref _searchKeyword, value);
        }

        public ObservableCollection<Allergen> Allergens
        {
            get => _allergens;
            set => SetProperty(ref _allergens, value);
        }

        public Allergen SelectedAllergen
        {
            get => _selectedAllergen;
            set => SetProperty(ref _selectedAllergen, value);
        }

        public bool ExcludeAllergen
        {
            get => _excludeAllergen;
            set => SetProperty(ref _excludeAllergen, value);
        }

        public ObservableCollection<Dish> DishResults
        {
            get => _dishResults;
            set => SetProperty(ref _dishResults, value);
        }

        public ObservableCollection<Menu> MenuResults
        {
            get => _menuResults;
            set => SetProperty(ref _menuResults, value);
        }

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetProperty(ref _selectedMenuItem, value);
        }

        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }

        public int TotalResults
        {
            get => _totalResults;
            set => SetProperty(ref _totalResults, value);
        }

        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        public bool IsClient => _userService.IsClient;
        public User CurrentUser => _userService.CurrentUser;

        public ICommand SearchCommand { get; private set; }
        public ICommand ClearSearchCommand { get; private set; }
        public ICommand AddToCartCommand { get; private set; }

        public SearchViewModel(SearchService searchService, AllergenService allergenService, UserService userService)
        {
            _searchService = searchService;
            _allergenService = allergenService;
            _userService = userService;

            Title = "Search Menu";
            Allergens = new ObservableCollection<Allergen>();
            DishResults = new ObservableCollection<Dish>();
            MenuResults = new ObservableCollection<Menu>();
            ExcludeAllergen = false;

            SearchCommand = new RelayCommand(async () => await SearchAsync());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            AddToCartCommand = new RelayCommand<MenuItem>(AddToCart, CanAddToCart);
            
            // Load allergens
            LoadAllergensAsync().ConfigureAwait(false);
        }

        private async Task LoadAllergensAsync()
        {
            try
            {
                IsSearching = true;
                
                var allergens = await _allergenService.GetAllAllergensAsync();
                Allergens.Clear();
                
                foreach (var allergen in allergens)
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
                IsSearching = false;
            }
        }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedAllergen == null)
            {
                ErrorMessage = "Please enter a search keyword or select an allergen.";
                return;
            }

            try
            {
                IsSearching = true;
                
                SearchResults results;
                
                if (SelectedAllergen != null)
                {
                    // Search by allergen
                    if (ExcludeAllergen)
                    {
                        // Search for items WITHOUT the selected allergen
                        results = await _searchService.GetFoodItemsWithoutAllergenAsync(SelectedAllergen.AllergenId);
                    }
                    else
                    {
                        // Search for items WITH the selected allergen
                        results = await _searchService.GetFoodItemsWithAllergenAsync(SelectedAllergen.AllergenId);
                    }
                    
                    // If a keyword is also provided, filter the results
                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        // Filter the results by keyword
                        var keyword = SearchKeyword.ToLower();
                        results.Dishes = new ObservableCollection<Dish>(
                            results.Dishes.Where(d => d.Name.ToLower().Contains(keyword) || 
                                                    (d.Description != null && d.Description.ToLower().Contains(keyword)))
                        );
                        
                        results.Menus = new ObservableCollection<Menu>(
                            results.Menus.Where(m => m.Name.ToLower().Contains(keyword) || 
                                                   (m.Description != null && m.Description.ToLower().Contains(keyword)))
                        );
                    }
                }
                else
                {
                    // Search by keyword only
                    results = await _searchService.SearchFoodItemsAsync(SearchKeyword);
                }
                
                // Update results
                DishResults = results.Dishes;
                MenuResults = results.Menus;
                TotalResults = results.TotalCount;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Search failed: {ex.Message}";
            }
            finally
            {
                IsSearching = false;
            }
        }

        private void ClearSearch()
        {
            SearchKeyword = string.Empty;
            SelectedAllergen = null;
            ExcludeAllergen = false;
            DishResults.Clear();
            MenuResults.Clear();
            TotalResults = 0;
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