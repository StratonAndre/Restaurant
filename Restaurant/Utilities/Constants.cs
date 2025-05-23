namespace RestaurantManager.Utilities
{
    /// <summary>
    /// Application constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Database connection string name in the configuration.
        /// </summary>
        public const string ConnectionStringName = "RestaurantDB";

        /// <summary>
        /// Configuration setting keys.
        /// </summary>
        public static class ConfigKeys
        {
            /// <summary>
            /// The menu discount percentage key.
            /// </summary>
            public const string MenuDiscountPercentage = "MenuDiscountPercentage";

            /// <summary>
            /// The minimum order amount for free delivery key.
            /// </summary>
            public const string MinOrderForFreeDelivery = "MinOrderForFreeDelivery";

            /// <summary>
            /// The standard delivery cost key.
            /// </summary>
            public const string StandardDeliveryCost = "StandardDeliveryCost";

            /// <summary>
            /// The order discount threshold key.
            /// </summary>
            public const string OrderDiscountThreshold = "OrderDiscountThreshold";

            /// <summary>
            /// The order discount percentage key.
            /// </summary>
            public const string OrderDiscountPercentage = "OrderDiscountPercentage";

            /// <summary>
            /// The frequent order count key.
            /// </summary>
            public const string FrequentOrderCount = "FrequentOrderCount";

            /// <summary>
            /// The frequent order period in days key.
            /// </summary>
            public const string FrequentOrderPeriodDays = "FrequentOrderPeriodDays";

            /// <summary>
            /// The frequent order discount percentage key.
            /// </summary>
            public const string FrequentOrderDiscount = "FrequentOrderDiscount";

            /// <summary>
            /// The low stock threshold key.
            /// </summary>
            public const string LowStockThreshold = "LowStockThreshold";
        }

        /// <summary>
        /// Application paths.
        /// </summary>
        public static class Paths
        {
            /// <summary>
            /// The images directory.
            /// </summary>
            public const string ImagesDirectory = "Resources/Images";
        }
    }
}