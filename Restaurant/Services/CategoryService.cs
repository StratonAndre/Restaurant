using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public class CategoryService
    {
        private readonly DatabaseService _databaseService;

        public CategoryService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get all categories
        public async Task<ObservableCollection<Category>> GetAllCategoriesAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllCategories");

                ObservableCollection<Category> categories = new ObservableCollection<Category>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        categories.Add(MapCategoryFromDataRow(row));
                    }
                }
                
                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all categories error: {ex.Message}");
                return new ObservableCollection<Category>();
            }
        }

        // Get a category by ID
        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CategoryId", categoryId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetCategoryById", parameters);

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return MapCategoryFromDataRow(result.Tables[0].Rows[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get category by ID error: {ex.Message}");
                return null;
            }
        }

        // Add a new category
        public async Task<Category> AddCategoryAsync(Category category)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Name", category.Name },
                    { "Description", category.Description }
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "CategoryId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_InsertCategory", parameters, outputParameters);

                if (result != null && result.ContainsKey("CategoryId"))
                {
                    category.CategoryId = Convert.ToInt32(result["CategoryId"]);
                    return category;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add category error: {ex.Message}");
                return null;
            }
        }

        // Update a category
        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CategoryId", category.CategoryId },
                    { "Name", category.Name },
                    { "Description", category.Description }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateCategory", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update category error: {ex.Message}");
                return false;
            }
        }

        // Delete a category
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "CategoryId", categoryId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_DeleteCategory", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete category error: {ex.Message}");
                return false;
            }
        }

        // Helper method to map a DataRow to a Category object
        private Category MapCategoryFromDataRow(DataRow row)
        {
            return new Category
            {
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString()
            };
        }
    }
}