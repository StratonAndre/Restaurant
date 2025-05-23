-- 19. Get Order Statistics (For Dashboard)
CREATE OR ALTER PROCEDURE sp_GetOrderStatistics
AS
BEGIN
    SET NOCOUNT ON;

    -- Total orders
    DECLARE @TotalOrders INT;
    SELECT @TotalOrders = COUNT(*) FROM Orders;

    -- Active orders
    DECLARE @ActiveOrderCount INT;
    SELECT @ActiveOrderCount = COUNT(*)
    FROM Orders
    WHERE StatusId NOT IN (
        -- Assuming status IDs for Delivered and Cancelled are 4 and 5 respectively
        SELECT StatusId FROM OrderStatus WHERE StatusName IN ('Delivered', 'Cancelled')
    );

    -- Today's revenue
    DECLARE @TodayRevenue DECIMAL(10, 2);
    SELECT @TodayRevenue = ISNULL(SUM(TotalCost), 0)
    FROM Orders
    WHERE CONVERT(DATE, OrderDate) = CONVERT(DATE, GETDATE())
      AND StatusId NOT IN (
        -- Exclude cancelled orders
        SELECT StatusId FROM OrderStatus WHERE StatusName = 'Cancelled'
    );

    -- This week's revenue
    DECLARE @WeekRevenue DECIMAL(10, 2);
    SELECT @WeekRevenue = ISNULL(SUM(TotalCost), 0)
    FROM Orders
    WHERE OrderDate >= DATEADD(DAY, -7, GETDATE())
      AND StatusId NOT IN (
        -- Exclude cancelled orders
        SELECT StatusId FROM OrderStatus WHERE StatusName = 'Cancelled'
    );

    -- This month's revenue
    DECLARE @MonthRevenue DECIMAL(10, 2);
    SELECT @MonthRevenue = ISNULL(SUM(TotalCost), 0)
    FROM Orders
    WHERE OrderDate >= DATEADD(MONTH, -1, GETDATE())
      AND StatusId NOT IN (
        -- Exclude cancelled orders
        SELECT StatusId FROM OrderStatus WHERE StatusName = 'Cancelled'
    );

    -- Most popular dishes (Fixed: using DishName instead of Name)
    SELECT TOP 5 d.DishId, d.DishName, COUNT(*) AS OrderCount
    FROM OrderDetails od
             JOIN Dishes d ON od.DishId = d.DishId
    GROUP BY d.DishId, d.DishName
    ORDER BY OrderCount DESC;

    -- Return statistics in a single row
    SELECT
        @TotalOrders AS TotalOrders,
        @ActiveOrderCount AS ActiveOrderCount,
        @TodayRevenue AS TodayRevenue,
        @WeekRevenue AS WeekRevenue,
        @MonthRevenue AS MonthRevenue;
END
GO

-- 25. Get All Settings
CREATE OR ALTER PROCEDURE sp_GetAllSettings
AS
BEGIN
    SET NOCOUNT ON;

    SELECT SettingID, SettingName, SettingValue
    FROM AppSettings
    ORDER BY SettingName;
END
GO

-- 26. Update Setting
CREATE OR ALTER PROCEDURE sp_UpdateSetting
    @SettingName NVARCHAR(100),
    @SettingValue NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE AppSettings
    SET SettingValue = @SettingValue
    WHERE SettingName = @SettingName;

    IF @@ROWCOUNT = 0
        BEGIN
            -- Setting doesn't exist, insert it
            INSERT INTO AppSettings (SettingName, SettingValue)
            VALUES (@SettingName, @SettingValue);
        END
END
GO