using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public class AllergenService
    {
        private readonly DatabaseService _databaseService;

        public AllergenService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get all allergens
        public async Task<ObservableCollection<Allergen>> GetAllAllergensAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllAllergens");

                ObservableCollection<Allergen> allergens = new ObservableCollection<Allergen>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        allergens.Add(MapAllergenFromDataRow(row));
                    }
                }
                
                return allergens;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all allergens error: {ex.Message}");
                return new ObservableCollection<Allergen>();
            }
        }

        // Get an allergen by ID
        public async Task<Allergen> GetAllergenByIdAsync(int allergenId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "AllergenId", allergenId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllergenById", parameters);

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return MapAllergenFromDataRow(result.Tables[0].Rows[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get allergen by ID error: {ex.Message}");
                return null;
            }
        }

        // Add a new allergen
        public async Task<Allergen> AddAllergenAsync(Allergen allergen)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Name", allergen.Name }  // Stored proc expects @Name
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "AllergenId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_InsertAllergen", parameters, outputParameters);

                if (result != null && result.ContainsKey("AllergenId"))
                {
                    allergen.AllergenId = Convert.ToInt32(result["AllergenId"]);
                    return allergen;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add allergen error: {ex.Message}");
                return null;
            }
        }

        // Update an allergen
        public async Task<bool> UpdateAllergenAsync(Allergen allergen)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "AllergenId", allergen.AllergenId },
                    { "Name", allergen.Name }  // Stored proc expects @Name
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateAllergen", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update allergen error: {ex.Message}");
                return false;
            }
        }

        // Delete an allergen
        public async Task<bool> DeleteAllergenAsync(int allergenId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "AllergenId", allergenId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_DeleteAllergen", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete allergen error: {ex.Message}");
                return false;
            }
        }

        // Get allergens for a specific dish
        public async Task<ObservableCollection<Allergen>> GetAllergensForDishAsync(int dishId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dishId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllergensForDish", parameters);

                ObservableCollection<Allergen> allergens = new ObservableCollection<Allergen>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        allergens.Add(MapAllergenFromDataRow(row));
                    }
                }
                
                return allergens;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get allergens for dish error: {ex.Message}");
                return new ObservableCollection<Allergen>();
            }
        }

        // Add an allergen to a dish
        public async Task<bool> AddAllergenToDishAsync(int dishId, int allergenId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dishId },
                    { "AllergenId", allergenId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_AddAllergenToDish", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add allergen to dish error: {ex.Message}");
                return false;
            }
        }

        // Remove an allergen from a dish
        public async Task<bool> RemoveAllergenFromDishAsync(int dishId, int allergenId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DishId", dishId },
                    { "AllergenId", allergenId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_RemoveAllergenFromDish", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Remove allergen from dish error: {ex.Message}");
                return false;
            }
        }

        // Helper method to map a DataRow to an Allergen object
        // FIXED: Map AllergenName column to Name property
        private Allergen MapAllergenFromDataRow(DataRow row)
        {
            return new Allergen
            {
                AllergenId = Convert.ToInt32(row["AllergenID"]),
                Name = row["AllergenName"].ToString(),  // Map AllergenName column to Name property
                Description = row.Table.Columns.Contains("Description") ? row["Description"]?.ToString() : string.Empty
            };
        }
    }
}