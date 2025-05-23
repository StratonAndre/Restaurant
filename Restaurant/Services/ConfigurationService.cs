using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public class ConfigurationService
    {
        private readonly DatabaseService _databaseService;
        private Dictionary<string, AppSetting> _settings;

        public ConfigurationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _settings = new Dictionary<string, AppSetting>();
        }

        // Load all application settings from the database
        public async Task LoadSettingsAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllSettings");

                if (result != null && result.Tables.Count > 0)
                {
                    _settings.Clear();
                    
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var setting = new AppSetting
                        {
                            SettingId = Convert.ToInt32(row["SettingID"]),
                            SettingKey = row["SettingName"].ToString(),  // Map SettingName column to SettingKey property
                            SettingValue = row["SettingValue"].ToString(),
                            Description = row.Table.Columns.Contains("Description") ? row["Description"]?.ToString() : string.Empty
                        };
                        
                        _settings[setting.SettingKey] = setting;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load settings error: {ex.Message}");
            }
        }

        // Get a setting by key
        public AppSetting GetSetting(string key)
        {
            return _settings.ContainsKey(key) ? _settings[key] : null;
        }

        // Get a decimal setting value
        public decimal GetDecimalSetting(string key, decimal defaultValue = 0)
        {
            if (_settings.ContainsKey(key))
            {
                return _settings[key].GetDecimalValue();
            }
            
            return defaultValue;
        }

        // Get an integer setting value
        public int GetIntSetting(string key, int defaultValue = 0)
        {
            if (_settings.ContainsKey(key))
            {
                return _settings[key].GetIntValue();
            }
            
            return defaultValue;
        }

        // Get a boolean setting value
        public bool GetBoolSetting(string key, bool defaultValue = false)
        {
            if (_settings.ContainsKey(key))
            {
                return _settings[key].GetBoolValue();
            }
            
            return defaultValue;
        }

        // Update a setting
        public async Task<bool> UpdateSettingAsync(string key, string value)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "SettingName", key },  // Stored proc expects @SettingName
                    { "SettingValue", value }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateSetting", parameters);

                if (rowsAffected > 0)
                {
                    // Update the in-memory setting
                    if (_settings.ContainsKey(key))
                    {
                        _settings[key].SettingValue = value;
                    }
                    
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update setting error: {ex.Message}");
                return false;
            }
        }

        // Get all the settings
        public List<AppSetting> GetAllSettings()
        {
            return _settings.Values.ToList();
        }

        // The specific settings required by the application

        // Menu discount percentage
        public decimal MenuDiscountPercentage => 
            GetDecimalSetting(AppSettingKeys.MenuDiscountPercentage, 10);

        // Minimum order amount for free delivery
        public decimal MinOrderForFreeDelivery => 
            GetDecimalSetting(AppSettingKeys.MinOrderForFreeDelivery, 100);

        // Standard delivery cost
        public decimal StandardDeliveryCost => 
            GetDecimalSetting(AppSettingKeys.StandardDeliveryCost, 15);

        // Order discount threshold
        public decimal OrderDiscountThreshold => 
            GetDecimalSetting(AppSettingKeys.OrderDiscountThreshold, 200);

        // Order discount percentage
        public decimal OrderDiscountPercentage => 
            GetDecimalSetting(AppSettingKeys.OrderDiscountPercentage, 5);

        // Frequent order count
        public int FrequentOrderCount => 
            GetIntSetting(AppSettingKeys.FrequentOrderCount, 3);

        // Frequent order period in days
        public int FrequentOrderPeriodDays => 
            GetIntSetting(AppSettingKeys.FrequentOrderPeriodDays, 30);

        // Frequent order discount percentage
        public decimal FrequentOrderDiscount => 
            GetDecimalSetting(AppSettingKeys.FrequentOrderDiscount, 7);

        // Low stock threshold
        public decimal LowStockThreshold => 
            GetDecimalSetting(AppSettingKeys.LowStockThreshold, 2);
    }
}