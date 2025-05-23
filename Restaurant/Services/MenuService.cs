using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public class MenuService
    {
        private readonly DatabaseService _databaseService;
        private readonly DishService _dishService;
        private readonly ConfigurationService _configurationService;

        public MenuService(DatabaseService databaseService, DishService dishService, ConfigurationService configurationService)
        {
            _databaseService = databaseService;
            _dishService = dishService;
            _configurationService = configurationService;
        }

        // Get all menus
        public async Task<ObservableCollection<Menu>> GetAllMenusAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllMenus");

                ObservableCollection<Menu> menus = new ObservableCollection<Menu>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var menu = MapMenuFromDataRow(row);
                        
                        // Get dishes for this menu
                        menu.MenuDishes = await GetDishesForMenuAsync(menu.Id);
                        
                        // Set the discount percentage from configuration
                        menu.DiscountPercentage = _configurationService.MenuDiscountPercentage;
                        
                        menus.Add(menu);
                    }
                }
                
                return menus;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all menus error: {ex.Message}");
                return new ObservableCollection<Menu>();
            }
        }

        // Get menus by category
        public async Task<ObservableCollection<Menu>> GetMenusByCategoryAsync(int categoryId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CategoryId", categoryId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetMenusByCategory", parameters);

                ObservableCollection<Menu> menus = new ObservableCollection<Menu>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var menu = MapMenuFromDataRow(row);
                        
                        // Get dishes for this menu
                        menu.MenuDishes = await GetDishesForMenuAsync(menu.Id);
                        
                        // Set the discount percentage from configuration
                        menu.DiscountPercentage = _configurationService.MenuDiscountPercentage;
                        
                        menus.Add(menu);
                    }
                }
                
                return menus;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get menus by category error: {ex.Message}");
                return new ObservableCollection<Menu>();
            }
        }

        // Get a menu by ID
        public async Task<Menu> GetMenuByIdAsync(int menuId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "MenuId", menuId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetMenuDetails", parameters);

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    var menu = MapMenuFromDataRow(result.Tables[0].Rows[0]);
                    
                    // Get dishes for this menu
                    menu.MenuDishes = await GetDishesForMenuAsync(menu.Id);
                    
                    // Set the discount percentage from configuration
                    menu.DiscountPercentage = _configurationService.MenuDiscountPercentage;
                    
                    return menu;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get menu by ID error: {ex.Message}");
                return null;
            }
        }

        // Add a new menu
        public async Task<Menu> AddMenuAsync(Menu menu)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CategoryId", menu.CategoryId },
                    { "Name", menu.Name },
                    { "Description", menu.Description },
                    { "DiscountPercentage", menu.DiscountPercentage }
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "MenuId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_InsertMenu", parameters, outputParameters);

                if (result != null && result.ContainsKey("MenuId"))
                {
                    menu.Id = Convert.ToInt32(result["MenuId"]);
                    
                    // Add dishes to this menu if any
                    if (menu.MenuDishes != null)
                    {
                        foreach (var menuDish in menu.MenuDishes)
                        {
                            menuDish.MenuId = menu.Id;
                            await AddDishToMenuAsync(menuDish);
                        }
                    }
                    
                    return menu;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add menu error: {ex.Message}");
                return null;
            }
        }

        // Update a menu
        public async Task<bool> UpdateMenuAsync(Menu menu)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "MenuId", menu.Id },
                    { "CategoryId", menu.CategoryId },
                    { "Name", menu.Name },
                    { "Description", menu.Description },
                    { "DiscountPercentage", menu.DiscountPercentage }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateMenu", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update menu error: {ex.Message}");
                return false;
            }
        }

        // Delete a menu
        public async Task<bool> DeleteMenuAsync(int menuId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "MenuId", menuId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_DeleteMenu", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete menu error: {ex.Message}");
                return false;
            }
        }

        // Add a dish to a menu
        public async Task<MenuDish> AddDishToMenuAsync(MenuDish menuDish)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "MenuId", menuDish.MenuId },
                    { "DishId", menuDish.DishId },
                    { "CustomQuantity", menuDish.CustomQuantity }
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "MenuDishId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_AddDishToMenu", parameters, outputParameters);

                if (result != null && result.ContainsKey("MenuDishId"))
                {
                    menuDish.MenuDishId = Convert.ToInt32(result["MenuDishId"]);
                    return menuDish;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add dish to menu error: {ex.Message}");
                return null;
            }
        }

        // Remove a dish from a menu
        public async Task<bool> RemoveDishFromMenuAsync(int menuDishId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "MenuDishId", menuDishId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_RemoveDishFromMenu", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Remove dish from menu error: {ex.Message}");
                return false;
            }
        }

        // Get dishes for a specific menu
        private async Task<ObservableCollection<MenuDish>> GetDishesForMenuAsync(int menuId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "MenuId", menuId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetDishesForMenu", parameters);

                ObservableCollection<MenuDish> menuDishes = new ObservableCollection<MenuDish>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        int dishId = Convert.ToInt32(row["DishId"]);
                        
                        var dish = await _dishService.GetDishByIdAsync(dishId);
                        
                        if (dish != null)
                        {
                            menuDishes.Add(new MenuDish
                            {
                                MenuDishId = Convert.ToInt32(row["MenuDishId"]),
                                MenuId = menuId,
                                DishId = dishId,
                                Dish = dish,
                                CustomQuantity = Convert.ToInt32(row["CustomQuantity"])
                            });
                        }
                    }
                }
                
                return menuDishes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get dishes for menu error: {ex.Message}");
                return new ObservableCollection<MenuDish>();
            }
        }

        // Helper method to map a DataRow to a Menu object
        private Menu MapMenuFromDataRow(DataRow row)
        {
            return new Menu
            {
                Id = Convert.ToInt32(row["MenuId"]),
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString()
            };
        }

        // Search menus by criteria
        public async Task<ObservableCollection<Menu>> SearchMenusAsync(string keyword, int? allergenId = null, bool excludeAllergen = false)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Keyword", keyword },
                    { "AllergenId", allergenId },
                    { "ExcludeAllergen", excludeAllergen }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_SearchMenus", parameters);

                ObservableCollection<Menu> menus = new ObservableCollection<Menu>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var menu = MapMenuFromDataRow(row);
                        
                        // Get dishes for this menu
                        menu.MenuDishes = await GetDishesForMenuAsync(menu.Id);
                        
                        // Set the discount percentage from configuration
                        menu.DiscountPercentage = _configurationService.MenuDiscountPercentage;
                        
                        menus.Add(menu);
                    }
                }
                
                return menus;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search menus error: {ex.Message}");
                return new ObservableCollection<Menu>();
            }
        }
    }
}