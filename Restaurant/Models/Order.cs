using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public class Order : INotifyPropertyChanged
    {
        private int _orderId;
        private string _orderCode;
        private int _userId;
        private DateTime _orderDate;
        private DateTime _estimatedDeliveryTime;
        private int _statusId;
        private decimal _foodCost;
        private decimal _deliveryCost;
        private decimal _discountAmount;
        private decimal _totalCost;
        private User _user;
        private OrderStatus _status;
        private ObservableCollection<OrderDetail> _orderDetails;

        public int OrderId
        {
            get => _orderId;
            set
            {
                if (_orderId != value)
                {
                    _orderId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OrderCode
        {
            get => _orderCode;
            set
            {
                if (_orderCode != value)
                {
                    _orderCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set
            {
                if (_orderDate != value)
                {
                    _orderDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime EstimatedDeliveryTime
        {
            get => _estimatedDeliveryTime;
            set
            {
                if (_estimatedDeliveryTime != value)
                {
                    _estimatedDeliveryTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public int StatusId
        {
            get => _statusId;
            set
            {
                if (_statusId != value)
                {
                    _statusId = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsActive));
                    OnPropertyChanged(nameof(CanBeCancelled));
                }
            }
        }

        public decimal FoodCost
        {
            get => _foodCost;
            set
            {
                if (_foodCost != value)
                {
                    _foodCost = value;
                    OnPropertyChanged();
                    CalculateTotalCost();
                }
            }
        }

        public decimal DeliveryCost
        {
            get => _deliveryCost;
            set
            {
                if (_deliveryCost != value)
                {
                    _deliveryCost = value;
                    OnPropertyChanged();
                    CalculateTotalCost();
                }
            }
        }

        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                if (_discountAmount != value)
                {
                    _discountAmount = value;
                    OnPropertyChanged();
                    CalculateTotalCost();
                }
            }
        }

        public decimal TotalCost
        {
            get => _totalCost;
            set  // Make this public instead of private
            {
                if (_totalCost != value)
                {
                    _totalCost = value;
                    OnPropertyChanged();
                }
            }
        }

        public User User
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    UserId = value?.UserId ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public OrderStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusId = value?.StatusId ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set
            {
                if (_orderDetails != value)
                {
                    if (_orderDetails != null)
                    {
                        // Detach event handlers from old collection
                        foreach (var detail in _orderDetails)
                        {
                            detail.PropertyChanged -= OrderDetail_PropertyChanged;
                        }
                        _orderDetails.CollectionChanged -= OrderDetails_CollectionChanged;
                    }

                    _orderDetails = value;

                    if (_orderDetails != null)
                    {
                        // Attach event handlers to new collection
                        foreach (var detail in _orderDetails)
                        {
                            detail.PropertyChanged += OrderDetail_PropertyChanged;
                        }
                        _orderDetails.CollectionChanged += OrderDetails_CollectionChanged;
                    }

                    CalculateFoodCost();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive => StatusId != (int)OrderStatusType.Delivered && StatusId != (int)OrderStatusType.Cancelled;

        public bool CanBeCancelled => StatusId == (int)OrderStatusType.Registered || StatusId == (int)OrderStatusType.InPreparation;

        public Order()
        {
            OrderDate = DateTime.Now;
            EstimatedDeliveryTime = DateTime.Now.AddHours(1);
            StatusId = (int)OrderStatusType.Registered;
            OrderDetails = new ObservableCollection<OrderDetail>();
            GenerateOrderCode();
        }

        private void GenerateOrderCode()
        {
            // Generate a unique order code (e.g., ORD-20240521-001)
            OrderCode = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        // Event handlers for OrderDetails collection changes
        private void OrderDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OrderDetail detail in e.NewItems)
                {
                    detail.PropertyChanged += OrderDetail_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (OrderDetail detail in e.OldItems)
                {
                    detail.PropertyChanged -= OrderDetail_PropertyChanged;
                }
            }

            CalculateFoodCost();
        }

        private void OrderDetail_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderDetail.Quantity) ||
                e.PropertyName == nameof(OrderDetail.UnitPrice) ||
                e.PropertyName == nameof(OrderDetail.MenuItem))
            {
                CalculateFoodCost();
            }
        }

        // Calculate the food cost based on order details
        private void CalculateFoodCost()
        {
            FoodCost = OrderDetails?.Sum(od => od.Quantity * od.UnitPrice) ?? 0;
        }

        // Calculate the total cost of the order
        private void CalculateTotalCost()
        {
            TotalCost = FoodCost + DeliveryCost - DiscountAmount;
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}