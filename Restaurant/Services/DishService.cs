using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public partial class DishService
    {
        private readonly DatabaseService _databaseService;
        private readonly AllergenService _allergenService;

        public DishService(DatabaseService databaseService, AllergenService allergenService)
        {
            _databaseService = databaseService;
            _allergenService = allergenService;
        }

        // Get all dishes
        public async Task<ObservableCollection<Dish>> GetAllDishesAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllDishes");

                ObservableCollection<Dish> dishes = new ObservableCollection<Dish>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var dish = MapDishFromDataRow(row);
                        
                        // Get allergens for this dish
                        dish.Allergens = await _allergenService.GetAllergensForDishAsync(dish.Id);
                        
                        // Get images for this dish
                        dish.Images = await GetImagesForDishAsync(dish.Id);
                        
                        dishes.Add(dish);
                    }
                }
                
                return dishes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all dishes error: {ex.Message}");
                return new ObservableCollection<Dish>();
            }
        }

        // Get dishes by category
        public async Task<ObservableCollection<Dish>> GetDishesByCategoryAsync(int categoryId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CategoryId", categoryId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetDishesByCategory", parameters);

                ObservableCollection<Dish> dishes = new ObservableCollection<Dish>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var dish = MapDishFromDataRow(row);
                        
                        // Get allergens for this dish
                        dish.Allergens = await _allergenService.GetAllergensForDishAsync(dish.Id);
                        
                        // Get images for this dish
                        dish.Images = await GetImagesForDishAsync(dish.Id);
                        
                        dishes.Add(dish);
                    }
                }
                
                return dishes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get dishes by category error: {ex.Message}");
                return new ObservableCollection<Dish>();
            }
        }

        // Get a dish by ID
        public async Task<Dish> GetDishByIdAsync(int dishId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dishId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetDishById", parameters);

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    var dish = MapDishFromDataRow(result.Tables[0].Rows[0]);
                    
                    // Get allergens for this dish
                    dish.Allergens = await _allergenService.GetAllergensForDishAsync(dish.Id);
                    
                    // Get images for this dish
                    dish.Images = await GetImagesForDishAsync(dish.Id);
                    
                    return dish;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get dish by ID error: {ex.Message}");
                return null;
            }
        }

        // Add a new dish
        public async Task<Dish> AddDishAsync(Dish dish)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CategoryId", dish.CategoryId },
                    { "Name", dish.Name },
                    { "Description", dish.Description },
                    { "PortionSize", dish.PortionSize },
                    { "Price", dish.Price },
                    { "TotalQuantity", dish.TotalQuantity },
                    { "IsAvailable", dish.IsAvailable }
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "DishId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_InsertDish", parameters, outputParameters);

                if (result != null && result.ContainsKey("DishId"))
                {
                    dish.Id = Convert.ToInt32(result["DishId"]);
                    return dish;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add dish error: {ex.Message}");
                return null;
            }
        }

        // Update a dish
        public async Task<bool> UpdateDishAsync(Dish dish)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dish.Id },
                    { "CategoryId", dish.CategoryId },
                    { "Name", dish.Name },
                    { "Description", dish.Description },
                    { "PortionSize", dish.PortionSize },
                    { "Price", dish.Price },
                    { "TotalQuantity", dish.TotalQuantity },
                    { "IsAvailable", dish.IsAvailable }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateDish", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update dish error: {ex.Message}");
                return false;
            }
        }

        // Delete a dish
        public async Task<bool> DeleteDishAsync(int dishId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dishId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_DeleteDish", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete dish error: {ex.Message}");
                return false;
            }
        }

        // Get low stock dishes
        public async Task<ObservableCollection<Dish>> GetLowStockDishesAsync(decimal threshold)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Threshold", threshold }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetLowStockDishes", parameters);

                ObservableCollection<Dish> dishes = new ObservableCollection<Dish>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var dish = MapDishFromDataRow(row);
                        dishes.Add(dish);
                    }
                }
                
                return dishes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get low stock dishes error: {ex.Message}");
                return new ObservableCollection<Dish>();
            }
        }

        // Get dishes by allergen
        public async Task<ObservableCollection<Dish>> GetDishesByAllergenAsync(int allergenId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "AllergenId", allergenId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetDishesByAllergen", parameters);

                ObservableCollection<Dish> dishes = new ObservableCollection<Dish>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var dish = MapDishFromDataRow(row);
                        
                        // Get allergens for this dish
                        dish.Allergens = await _allergenService.GetAllergensForDishAsync(dish.Id);
                        
                        // Get images for this dish
                        dish.Images = await GetImagesForDishAsync(dish.Id);
                        
                        dishes.Add(dish);
                    }
                }
                
                return dishes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get dishes by allergen error: {ex.Message}");
                return new ObservableCollection<Dish>();
            }
        }

        // Get images for a dish
        public async Task<ObservableCollection<DishImage>> GetImagesForDishAsync(int dishId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dishId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetImagesForDish", parameters);

                ObservableCollection<DishImage> images = new ObservableCollection<DishImage>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        images.Add(new DishImage
                        {
                            ImageId = Convert.ToInt32(row["ImageId"]),
                            DishId = Convert.ToInt32(row["DishId"]),
                            ImagePath = row["ImagePath"].ToString()
                        });
                    }
                }
                
                return images;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get images for dish error: {ex.Message}");
                return new ObservableCollection<DishImage>();
            }
        }

        // Add dish image
        public async Task<DishImage> AddDishImageAsync(DishImage image)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", image.DishId },
                    { "ImagePath", image.ImagePath }
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "ImageId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_AddDishImage", parameters, outputParameters);

                if (result != null && result.ContainsKey("ImageId"))
                {
                    image.ImageId = Convert.ToInt32(result["ImageId"]);
                    return image;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add dish image error: {ex.Message}");
                return null;
            }
        }

        // Delete dish image
        public async Task<bool> DeleteDishImageAsync(int imageId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "ImageId", imageId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_DeleteDishImage", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete dish image error: {ex.Message}");
                return false;
            }
        }

        // Helper method to map a DataRow to a Dish object
        private Dish MapDishFromDataRow(DataRow row)
        {
            return new Dish
            {
                Id = Convert.ToInt32(row["DishId"]),
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString(),
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                PortionSize = Convert.ToInt32(row["PortionSize"]),
                Price = Convert.ToDecimal(row["Price"]),
                TotalQuantity = Convert.ToDecimal(row["TotalQuantity"]),
                IsAvailable = Convert.ToBoolean(row["IsAvailable"])
            };
        }

        // Search dishes by criteria (from the partial class)
        public async Task<ObservableCollection<Dish>> SearchDishesAsync(string keyword, int? allergenId = null, bool excludeAllergen = false)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Keyword", keyword },
                    { "AllergenId", allergenId },
                    { "ExcludeAllergen", excludeAllergen }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_SearchDishes", parameters);

                ObservableCollection<Dish> dishes = new ObservableCollection<Dish>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var dish = MapDishFromDataRow(row);
                        
                        // Get allergens for this dish
                        dish.Allergens = await _allergenService.GetAllergensForDishAsync(dish.Id);
                        
                        // Get images for this dish
                        dish.Images = await GetImagesForDishAsync(dish.Id);
                        
                        dishes.Add(dish);
                    }
                }
                
                return dishes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search dishes error: {ex.Message}");
                return new ObservableCollection<Dish>();
            }
        }

        // Update dish quantity after a completed order
        public async Task<bool> UpdateDishQuantityAsync(int dishId, decimal quantityUsed)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dishId },
                    { "QuantityUsed", quantityUsed }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateDishQuantity", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update dish quantity error: {ex.Message}");
                return false;
            }
        }
    }
}