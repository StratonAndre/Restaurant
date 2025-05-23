using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public class AppSetting : INotifyPropertyChanged
    {
        private int _settingId;
        private string _settingKey;
        private string _settingValue;
        private string _description;

        public int SettingId
        {
            get => _settingId;
            set
            {
                if (_settingId != value)
                {
                    _settingId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SettingKey
        {
            get => _settingKey;
            set
            {
                if (_settingKey != value)
                {
                    _settingKey = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SettingValue
        {
            get => _settingValue;
            set
            {
                if (_settingValue != value)
                {
                    _settingValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        // Convert setting value to specific types
        public decimal GetDecimalValue()
        {
            if (decimal.TryParse(SettingValue, out decimal result))
                return result;
            return 0;
        }

        public int GetIntValue()
        {
            if (int.TryParse(SettingValue, out int result))
                return result;
            return 0;
        }

        public bool GetBoolValue()
        {
            if (bool.TryParse(SettingValue, out bool result))
                return result;
            return false;
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    // Constants for the AppSettings keys
    public static class AppSettingKeys
    {
        public const string MenuDiscountPercentage = "MenuDiscountPercentage";
        public const string MinOrderForFreeDelivery = "MinOrderForFreeDelivery";
        public const string StandardDeliveryCost = "StandardDeliveryCost";
        public const string OrderDiscountThreshold = "OrderDiscountThreshold";
        public const string OrderDiscountPercentage = "OrderDiscountPercentage";
        public const string FrequentOrderCount = "FrequentOrderCount";
        public const string FrequentOrderPeriodDays = "FrequentOrderPeriodDays";
        public const string FrequentOrderDiscount = "FrequentOrderDiscount";
        public const string LowStockThreshold = "LowStockThreshold";
    }
}