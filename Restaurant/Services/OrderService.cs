using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public partial class OrderService
    {
        private readonly DatabaseService _databaseService;
        private readonly DishService _dishService;
        private readonly MenuService _menuService;
        private readonly ConfigurationService _configurationService;

        public OrderService(DatabaseService databaseService, DishService dishService, 
            MenuService menuService, ConfigurationService configurationService)
        {
            _databaseService = databaseService;
            _dishService = dishService;
            _menuService = menuService;
            _configurationService = configurationService;
        }

        // Place a new order
        public async Task<Order> PlaceOrderAsync(Order order)
        {
            try
            {
                // Generate order code if not provided
                if (string.IsNullOrEmpty(order.OrderCode))
                {
                    order.OrderCode = GenerateOrderCode();
                }

                var parameters = new Dictionary<string, object>
                {
                    { "UserId", order.UserId },
                    { "OrderCode", order.OrderCode },
                    { "OrderDate", order.OrderDate },
                    { "EstimatedDeliveryTime", order.EstimatedDeliveryTime },
                    { "StatusId", order.StatusId },
                    { "FoodCost", order.FoodCost },
                    { "DeliveryCost", order.DeliveryCost },
                    { "DiscountAmount", order.DiscountAmount }
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "OrderId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_PlaceOrder", parameters, outputParameters);

                if (result != null && result.ContainsKey("OrderId"))
                {
                    order.OrderId = Convert.ToInt32(result["OrderId"]);
                    
                    // Add order details
                    foreach (var detail in order.OrderDetails)
                    {
                        detail.OrderId = order.OrderId;
                        await AddOrderDetailAsync(detail);
                    }
                    
                    return order;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Place order error: {ex.Message}");
                return null;
            }
        }

        // Add order detail
        private async Task<OrderDetail> AddOrderDetailAsync(OrderDetail detail)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "OrderId", detail.OrderId },
                    { "DishId", detail.DishId },
                    { "MenuId", detail.MenuId },
                    { "Quantity", detail.Quantity },
                    { "UnitPrice", detail.UnitPrice }
                };

                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "OrderDetailId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_AddOrderDetail", parameters, outputParameters);

                if (result != null && result.ContainsKey("OrderDetailId"))
                {
                    detail.OrderDetailId = Convert.ToInt32(result["OrderDetailId"]);
                    return detail;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add order detail error: {ex.Message}");
                return null;
            }
        }

        // Update order status
        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatusType status)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "OrderId", orderId },
                    { "StatusId", (int)status }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateOrderStatus", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update order status error: {ex.Message}");
                return false;
            }
        }

        // Cancel an order
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "OrderId", orderId }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_CancelOrder", parameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cancel order error: {ex.Message}");
                return false;
            }
        }

        // Get orders for a specific user
        public async Task<ObservableCollection<Order>> GetOrdersForUserAsync(int userId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "UserId", userId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetOrdersForUser", parameters);

                ObservableCollection<Order> orders = new ObservableCollection<Order>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var order = MapOrderFromDataRow(row);
                        
                        // Get order details
                        order.OrderDetails = await GetOrderDetailsAsync(order.OrderId);
                        
                        orders.Add(order);
                    }
                }
                
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get orders for user error: {ex.Message}");
                return new ObservableCollection<Order>();
            }
        }

        // Get all orders
        public async Task<ObservableCollection<Order>> GetAllOrdersAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetAllOrders");

                ObservableCollection<Order> orders = new ObservableCollection<Order>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var order = MapOrderFromDataRow(row);
                        
                        // Get order details
                        order.OrderDetails = await GetOrderDetailsAsync(order.OrderId);
                        
                        orders.Add(order);
                    }
                }
                
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all orders error: {ex.Message}");
                return new ObservableCollection<Order>();
            }
        }

        // Get active orders
        public async Task<ObservableCollection<Order>> GetActiveOrdersAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetActiveOrders");

                ObservableCollection<Order> orders = new ObservableCollection<Order>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var order = MapOrderFromDataRow(row);
                        
                        // Get order details
                        order.OrderDetails = await GetOrderDetailsAsync(order.OrderId);
                        
                        orders.Add(order);
                    }
                }
                
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get active orders error: {ex.Message}");
                return new ObservableCollection<Order>();
            }
        }

        // Get order by ID
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "OrderId", orderId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetOrderDetails", parameters);

                if (result != null && result.Tables.Count > 1 && result.Tables[0].Rows.Count > 0)
                {
                    // First table contains order information
                    var order = MapOrderFromDataRow(result.Tables[0].Rows[0]);
                    
                    // Second table contains order details
                    ObservableCollection<OrderDetail> orderDetails = new ObservableCollection<OrderDetail>();
                    
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        var detail = new OrderDetail
                        {
                            OrderDetailId = Convert.ToInt32(row["OrderDetailId"]),
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            DishId = row["DishId"] != DBNull.Value ? Convert.ToInt32(row["DishId"]) : (int?)null,
                            MenuId = row["MenuId"] != DBNull.Value ? Convert.ToInt32(row["MenuId"]) : (int?)null,
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            UnitPrice = Convert.ToDecimal(row["UnitPrice"])
                        };
                        
                        // Load the dish or menu
                        if (detail.DishId.HasValue)
                        {
                            detail.Dish = await _dishService.GetDishByIdAsync(detail.DishId.Value);
                        }
                        else if (detail.MenuId.HasValue)
                        {
                            detail.Menu = await _menuService.GetMenuByIdAsync(detail.MenuId.Value);
                        }
                        
                        orderDetails.Add(detail);
                    }
                    
                    order.OrderDetails = orderDetails;
                    return order;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get order by ID error: {ex.Message}");
                return null;
            }
        }

        // Get order details for a specific order
        private async Task<ObservableCollection<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "OrderId", orderId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetOrderDetails", parameters);

                ObservableCollection<OrderDetail> orderDetails = new ObservableCollection<OrderDetail>();
                
                if (result != null && result.Tables.Count > 1)
                {
                    // Second table contains order details
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        var detail = new OrderDetail
                        {
                            OrderDetailId = Convert.ToInt32(row["OrderDetailId"]),
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            DishId = row["DishId"] != DBNull.Value ? Convert.ToInt32(row["DishId"]) : (int?)null,
                            MenuId = row["MenuId"] != DBNull.Value ? Convert.ToInt32(row["MenuId"]) : (int?)null,
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            UnitPrice = Convert.ToDecimal(row["UnitPrice"])
                        };
                        
                        // Load the dish or menu
                        if (detail.DishId.HasValue)
                        {
                            detail.Dish = await _dishService.GetDishByIdAsync(detail.DishId.Value);
                        }
                        else if (detail.MenuId.HasValue)
                        {
                            detail.Menu = await _menuService.GetMenuByIdAsync(detail.MenuId.Value);
                        }
                        
                        orderDetails.Add(detail);
                    }
                }
                
                return orderDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get order details error: {ex.Message}");
                return new ObservableCollection<OrderDetail>();
            }
        }

        // Generate a unique order code
        private string GenerateOrderCode()
        {
            return $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        // Helper method to map a DataRow to an Order object
        private Order MapOrderFromDataRow(DataRow row)
        {
            return new Order
            {
                OrderId = Convert.ToInt32(row["OrderId"]),
                OrderCode = row["OrderCode"].ToString(),
                UserId = Convert.ToInt32(row["UserId"]),
                OrderDate = Convert.ToDateTime(row["OrderDate"]),
                EstimatedDeliveryTime = Convert.ToDateTime(row["EstimatedDeliveryTime"]),
                StatusId = Convert.ToInt32(row["StatusId"]),
                Status = new OrderStatus
                {
                    StatusId = Convert.ToInt32(row["StatusId"]),
                    StatusName = row["StatusName"].ToString()
                },
                FoodCost = Convert.ToDecimal(row["FoodCost"]),
                DeliveryCost = Convert.ToDecimal(row["DeliveryCost"]),
                DiscountAmount = Convert.ToDecimal(row["DiscountAmount"]),
                TotalCost = Convert.ToDecimal(row["TotalCost"]),
                User = row.Table.Columns.Contains("FirstName") && row.Table.Columns.Contains("LastName") ? 
                    new User
                    {
                        UserId = Convert.ToInt32(row["UserId"]),
                        FirstName = row["FirstName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        Email = row["Email"].ToString(),
                        PhoneNumber = row["PhoneNumber"].ToString(),
                        DeliveryAddress = row["DeliveryAddress"].ToString()
                    } : null
            };
        }

        // Get orders by status
        public async Task<ObservableCollection<Order>> GetOrdersByStatusAsync(OrderStatusType status)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "StatusId", (int)status }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetOrdersByStatus", parameters);

                ObservableCollection<Order> orders = new ObservableCollection<Order>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var order = MapOrderFromDataRow(row);
                        
                        // Get order details
                        order.OrderDetails = await GetOrderDetailsAsync(order.OrderId);
                        
                        orders.Add(order);
                    }
                }
                
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get orders by status error: {ex.Message}");
                return new ObservableCollection<Order>();
            }
        }

        // Get active orders for a specific user
        public async Task<ObservableCollection<Order>> GetActiveOrdersForUserAsync(int userId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "UserId", userId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetActiveOrdersForUser", parameters);

                ObservableCollection<Order> orders = new ObservableCollection<Order>();
                
                if (result != null && result.Tables.Count > 0)
                {
                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        var order = MapOrderFromDataRow(row);
                        
                        // Get order details
                        order.OrderDetails = await GetOrderDetailsAsync(order.OrderId);
                        
                        orders.Add(order);
                    }
                }
                
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get active orders for user error: {ex.Message}");
                return new ObservableCollection<Order>();
            }
        }

        // Get order statistics for dashboard
        public async Task<Dictionary<string, object>> GetOrderStatisticsAsync()
        {
            try
            {
                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetOrderStatistics");

                Dictionary<string, object> statistics = new Dictionary<string, object>();
                
                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataRow row = result.Tables[0].Rows[0];
                    
                    foreach (DataColumn column in result.Tables[0].Columns)
                    {
                        statistics[column.ColumnName] = row[column];
                    }
                }
                
                return statistics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get order statistics error: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }
    }
}