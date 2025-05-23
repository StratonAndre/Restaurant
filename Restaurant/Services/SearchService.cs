using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public class SearchService
    {
        private readonly DatabaseService _databaseService;
        private readonly AllergenService _allergenService;
        private readonly DishService _dishService;
        private readonly MenuService _menuService;

        public SearchService(DatabaseService databaseService, AllergenService allergenService, 
                           DishService dishService, MenuService menuService)
        {
            _databaseService = databaseService;
            _allergenService = allergenService;
            _dishService = dishService;
            _menuService = menuService;
        }

        // Search for food items (dishes and menus) based on keywords and allergen filters
        public async Task<SearchResults> SearchFoodItemsAsync(string keyword, int? allergenId = null, bool excludeAllergen = false)
        {
            try
            {
                // Search for dishes
                var dishes = await _dishService.SearchDishesAsync(keyword, allergenId, excludeAllergen);
                
                // Search for menus
                var menus = await _menuService.SearchMenusAsync(keyword, allergenId, excludeAllergen);
                
                return new SearchResults
                {
                    Dishes = dishes,
                    Menus = menus
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search food items error: {ex.Message}");
                return new SearchResults
                {
                    Dishes = new ObservableCollection<Dish>(),
                    Menus = new ObservableCollection<Menu>()
                };
            }
        }

        // Get food items that contain a specific allergen
        public async Task<SearchResults> GetFoodItemsWithAllergenAsync(int allergenId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "AllergenId", allergenId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetFoodItemsWithAllergen", parameters);

                var dishes = new ObservableCollection<Dish>();
                var menus = new ObservableCollection<Menu>();
                
                if (result != null && result.Tables.Count > 1)
                {
                    // Process dishes table
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        int dishId = Convert.ToInt32(row["DishId"]);
                        var dish = await _dishService.GetDishByIdAsync(dishId);
                        if (dish != null)
                        {
                            dishes.Add(dish);
                        }
                    }
                    
                    // Process menus table
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        int menuId = Convert.ToInt32(row["MenuId"]);
                        var menu = await _menuService.GetMenuByIdAsync(menuId);
                        if (menu != null)
                        {
                            menus.Add(menu);
                        }
                    }
                }
                
                return new SearchResults
                {
                    Dishes = dishes,
                    Menus = menus
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get food items with allergen error: {ex.Message}");
                return new SearchResults
                {
                    Dishes = new ObservableCollection<Dish>(),
                    Menus = new ObservableCollection<Menu>()
                };
            }
        }

        // Get food items that do NOT contain a specific allergen
        public async Task<SearchResults> GetFoodItemsWithoutAllergenAsync(int allergenId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "AllergenId", allergenId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetFoodItemsWithoutAllergen", parameters);

                var dishes = new ObservableCollection<Dish>();
                var menus = new ObservableCollection<Menu>();
                
                if (result != null && result.Tables.Count > 1)
                {
                    // Process dishes table
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        int dishId = Convert.ToInt32(row["DishId"]);
                        var dish = await _dishService.GetDishByIdAsync(dishId);
                        if (dish != null)
                        {
                            dishes.Add(dish);
                        }
                    }
                    
                    // Process menus table
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        int menuId = Convert.ToInt32(row["MenuId"]);
                        var menu = await _menuService.GetMenuByIdAsync(menuId);
                        if (menu != null)
                        {
                            menus.Add(menu);
                        }
                    }
                }
                
                return new SearchResults
                {
                    Dishes = dishes,
                    Menus = menus
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get food items without allergen error: {ex.Message}");
                return new SearchResults
                {
                    Dishes = new ObservableCollection<Dish>(),
                    Menus = new ObservableCollection<Menu>()
                };
            }
        }
    }

    // Class to hold search results
    public class SearchResults
    {
        public ObservableCollection<Dish> Dishes { get; set; }
        public ObservableCollection<Menu> Menus { get; set; }

        public int TotalCount => (Dishes?.Count ?? 0) + (Menus?.Count ?? 0);

        public SearchResults()
        {
            Dishes = new ObservableCollection<Dish>();
            Menus = new ObservableCollection<Menu>();
        }
    }
}