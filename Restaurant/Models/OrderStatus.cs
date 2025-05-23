using System.ComponentModel;

namespace RestaurantManager.Models
{
    /// <summary>
    /// Enumeration of possible order statuses.
    /// </summary>
    public enum OrderStatusType
    {
        /// <summary>
        /// The order has been registered in the system.
        /// </summary>
        [Description("Registered")]
        Registered = 1,

        /// <summary>
        /// The order is being prepared.
        /// </summary>
        [Description("In Preparation")]
        InPreparation = 2,

        /// <summary>
        /// The order has been sent out for delivery.
        /// </summary>
        [Description("Out for Delivery")]
        OutForDelivery = 3,

        /// <summary>
        /// The order has been delivered to the customer.
        /// </summary>
        [Description("Delivered")]
        Delivered = 4,

        /// <summary>
        /// The order has been cancelled.
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 5
    }

    /// <summary>
    /// Model for the OrderStatus entity.
    /// </summary>
    public class OrderStatus
    {
        /// <summary>
        /// Gets or sets the status identifier.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the name of the status (DB column: StatusName).
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Gets or sets the name (for compatibility with mapping code).
        /// </summary>
        public string Name
        {
            get => StatusName;
            set => StatusName = value;
        }

        /// <summary>
        /// Gets a value indicating whether this status represents an active order.
        /// </summary>
        public bool IsActive => StatusId != (int)OrderStatusType.Delivered && StatusId != (int)OrderStatusType.Cancelled;
    }
}