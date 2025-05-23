-- CORRECTED ORDER PROCEDURES
-- Fixed to match actual database schema without IsAvailable column

-- 1. Place Order (Complex - with transaction)
-- CORRECTED Place Order Procedure
-- Fixed to properly include DiscountAmount column

CREATE OR ALTER PROCEDURE sp_PlaceOrder
    @UserId INT,
    @OrderCode NVARCHAR(20),
    @OrderDate DATETIME,
    @EstimatedDeliveryTime DATETIME,
    @StatusId INT,
    @FoodCost DECIMAL(10, 2),
    @DeliveryCost DECIMAL(10, 2),
    @DiscountAmount DECIMAL(10, 2),
    @OrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Insert the order (TotalCost is calculated column)
        INSERT INTO Orders (OrderCode, UserID, OrderDate, EstimatedDeliveryTime, StatusID,
                            FoodCost, DeliveryCost, DiscountAmount)  -- ✅ 8 columns
        VALUES (@OrderCode, @UserId, @OrderDate, @EstimatedDeliveryTime, @StatusId,
                @FoodCost, @DeliveryCost, @DiscountAmount); 
        SET @OrderId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

-- 2. Add Order Detail
CREATE OR ALTER PROCEDURE sp_AddOrderDetail
    @OrderId INT,
    @DishId INT = NULL,
    @MenuId INT = NULL,
    @Quantity INT,
    @UnitPrice DECIMAL(10, 2),
    @OrderDetailId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Check if either DishId or MenuId is provided
        IF @DishId IS NULL AND @MenuId IS NULL
            BEGIN
                RAISERROR('Either DishId or MenuId must be provided', 16, 1);
                RETURN;
            END

        -- Insert order detail
        INSERT INTO OrderDetails (OrderID, DishID, MenuID, Quantity, UnitPrice)
        VALUES (@OrderId, @DishId, @MenuId, @Quantity, @UnitPrice);

        SET @OrderDetailId = SCOPE_IDENTITY();

        -- If this is a dish order, update the quantity in stock
        IF @DishId IS NOT NULL
            BEGIN
                -- Get the dish portion size
                DECLARE @PortionSize INT;
                SELECT @PortionSize = PortionQuantity FROM Dishes WHERE DishID = @DishId;

                -- Calculate total quantity to subtract (portion size * order quantity)
                DECLARE @TotalQuantityToSubtract DECIMAL(10, 2);
                SET @TotalQuantityToSubtract = @PortionSize * @Quantity / 1000.0; -- Convert from grams to kg

                -- Update dish quantity (availability is determined by TotalQuantity > 0)
                UPDATE Dishes
                SET TotalQuantity = CASE
                                        WHEN TotalQuantity - @TotalQuantityToSubtract < 0 THEN 0
                                        ELSE TotalQuantity - @TotalQuantityToSubtract
                    END
                WHERE DishID = @DishId;
            END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

-- 3. Update Order Status
CREATE OR ALTER PROCEDURE sp_UpdateOrderStatus
    @OrderId INT,
    @StatusId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Orders
    SET StatusID = @StatusId
    WHERE OrderID = @OrderId;

    SELECT @@ROWCOUNT;
END
GO

-- 4. Get Orders By User
CREATE OR ALTER PROCEDURE sp_GetOrdersForUser
@UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT o.*, os.StatusName
    FROM Orders o
             JOIN OrderStatus os ON o.StatusID = os.StatusID
    WHERE o.UserID = @UserId
    ORDER BY o.OrderDate DESC;
END
GO

-- 5. Get Active Orders For User
CREATE OR ALTER PROCEDURE sp_GetActiveOrdersForUser
@UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT o.*, os.StatusName
    FROM Orders o
             JOIN OrderStatus os ON o.StatusID = os.StatusID
    WHERE o.UserID = @UserId
      AND o.StatusID NOT IN (
        SELECT StatusID FROM OrderStatus WHERE StatusName IN ('Delivered', 'Cancelled')
    )
    ORDER BY o.OrderDate DESC;
END
GO

-- 6. Get All Orders (For Employees)
CREATE OR ALTER PROCEDURE sp_GetAllOrders
AS
BEGIN
    SET NOCOUNT ON;

    SELECT o.*, os.StatusName, u.FirstName, u.LastName, u.PhoneNumber, u.DeliveryAddress
    FROM Orders o
             JOIN OrderStatus os ON o.StatusID = os.StatusID
             JOIN Users u ON o.UserID = u.UserID
    ORDER BY o.OrderDate DESC;
END
GO

-- 7. Get Active Orders (For Employees)
CREATE OR ALTER PROCEDURE sp_GetActiveOrders
AS
BEGIN
    SET NOCOUNT ON;

    SELECT o.*, os.StatusName, u.FirstName, u.LastName, u.PhoneNumber, u.DeliveryAddress
    FROM Orders o
             JOIN OrderStatus os ON o.StatusID = os.StatusID
             JOIN Users u ON o.UserID = u.UserID
    WHERE o.StatusID NOT IN (
        SELECT StatusID FROM OrderStatus WHERE StatusName IN ('Delivered', 'Cancelled')
    )
    ORDER BY o.OrderDate DESC;
END
GO

-- 8. Get Order Details
CREATE OR ALTER PROCEDURE sp_GetOrderDetails
@OrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Get order information
    SELECT o.*, os.StatusName, u.FirstName, u.LastName, u.PhoneNumber, u.DeliveryAddress
    FROM Orders o
             JOIN OrderStatus os ON o.StatusID = os.StatusID
             JOIN Users u ON o.UserID = u.UserID
    WHERE o.OrderID = @OrderId;

    -- Get order details
    SELECT od.*,
           d.DishName AS DishName,
           m.MenuName AS MenuName,
           CASE
               WHEN od.DishID IS NOT NULL THEN 'Dish'
               ELSE 'Menu'
               END AS ItemType
    FROM OrderDetails od
             LEFT JOIN Dishes d ON od.DishID = d.DishID
             LEFT JOIN Menus m ON od.MenuID = m.MenuID
    WHERE od.OrderID = @OrderId;
END
GO

-- 9. Cancel Order
CREATE OR ALTER PROCEDURE sp_CancelOrder
@OrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Get order status
        DECLARE @CurrentStatusId INT;
        SELECT @CurrentStatusId = StatusID FROM Orders WHERE OrderID = @OrderId;

        -- Check if order can be cancelled (not already delivered or cancelled)
        IF @CurrentStatusId IN (
            SELECT StatusID FROM OrderStatus WHERE StatusName IN ('Delivered', 'Cancelled')
        )
            BEGIN
                RAISERROR('Order cannot be cancelled as it is already delivered or cancelled', 16, 1);
                RETURN;
            END

        -- Get cancelled status ID
        DECLARE @CancelledStatusId INT;
        SELECT @CancelledStatusId = StatusID FROM OrderStatus WHERE StatusName = 'Cancelled';

        -- Update order status to cancelled
        UPDATE Orders
        SET StatusID = @CancelledStatusId
        WHERE OrderID = @OrderId;

        -- Return any deducted quantities back to inventory
        -- Get order details with dishes
        DECLARE @OrderDetails TABLE (
                                        OrderDetailId INT,
                                        DishId INT,
                                        Quantity INT
                                    );

        INSERT INTO @OrderDetails (OrderDetailId, DishId, Quantity)
        SELECT od.OrderDetailID, od.DishID, od.Quantity
        FROM OrderDetails od
        WHERE od.OrderID = @OrderId AND od.DishID IS NOT NULL;

        -- Update dish quantities
        DECLARE @DetailId INT, @DishId INT, @Quantity INT, @PortionSize INT;
        DECLARE detail_cursor CURSOR FOR
            SELECT OrderDetailId, DishId, Quantity FROM @OrderDetails;

        OPEN detail_cursor;
        FETCH NEXT FROM detail_cursor INTO @DetailId, @DishId, @Quantity;

        WHILE @@FETCH_STATUS = 0
            BEGIN
                -- Get the dish portion size
                SELECT @PortionSize = PortionQuantity FROM Dishes WHERE DishID = @DishId;

                -- Calculate total quantity to add back (portion size * order quantity)
                DECLARE @TotalQuantityToAdd DECIMAL(10, 2);
                SET @TotalQuantityToAdd = @PortionSize * @Quantity / 1000.0; -- Convert from grams to kg

                -- Update dish quantity (no IsAvailable column, availability determined by TotalQuantity > 0)
                UPDATE Dishes
                SET TotalQuantity = TotalQuantity + @TotalQuantityToAdd
                WHERE DishID = @DishId;

                FETCH NEXT FROM detail_cursor INTO @DetailId, @DishId, @Quantity;
            END

        CLOSE detail_cursor;
        DEALLOCATE detail_cursor;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Re-throw the error
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END
GO

-- 10. Get Orders By Status
CREATE OR ALTER PROCEDURE sp_GetOrdersByStatus
@StatusId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT o.*, os.StatusName, u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.DeliveryAddress
    FROM Orders o
             JOIN OrderStatus os ON o.StatusID = os.StatusID
             JOIN Users u ON o.UserID = u.UserID
    WHERE o.StatusID = @StatusId
    ORDER BY o.OrderDate DESC;
END
GO