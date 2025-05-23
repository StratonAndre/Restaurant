-- CORRECTED DISH PROCEDURES
-- Fixed to match actual database schema without Description and IsAvailable columns

-- Get All Dishes
CREATE OR ALTER PROCEDURE sp_GetAllDishes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
           d.Price, d.TotalQuantity,
           c.CategoryName
    FROM Dishes d
             LEFT JOIN Categories c ON d.CategoryID = c.CategoryID
    ORDER BY d.DishName;
END
GO

-- Get Dish By ID
CREATE OR ALTER PROCEDURE sp_GetDishById
@DishId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
           d.Price, d.TotalQuantity,
           c.CategoryName
    FROM Dishes d
             LEFT JOIN Categories c ON d.CategoryID = c.CategoryID
    WHERE d.DishID = @DishId;
END
GO

-- Get Dishes By Category
CREATE OR ALTER PROCEDURE sp_GetDishesByCategory
@CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
           d.Price, d.TotalQuantity
    FROM Dishes d
    WHERE d.CategoryID = @CategoryId
    ORDER BY d.DishName;
END
GO

-- Insert Dish
CREATE OR ALTER PROCEDURE sp_InsertDish
    @CategoryId INT,
    @Name NVARCHAR(100),
    @PortionSize INT,
    @Price DECIMAL(10, 2),
    @TotalQuantity DECIMAL(10, 2),
    @DishId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Dishes (CategoryID, DishName, PortionQuantity, Price, TotalQuantity)
    VALUES (@CategoryId, @Name, @PortionSize, @Price, @TotalQuantity);

    SET @DishId = SCOPE_IDENTITY();
END
GO

-- Update Dish
CREATE OR ALTER PROCEDURE sp_UpdateDish
    @DishId INT,
    @CategoryId INT,
    @Name NVARCHAR(100),
    @PortionSize INT,
    @Price DECIMAL(10, 2),
    @TotalQuantity DECIMAL(10, 2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Dishes
    SET CategoryID = @CategoryId,
        DishName = @Name,
        PortionQuantity = @PortionSize,
        Price = @Price,
        TotalQuantity = @TotalQuantity
    WHERE DishID = @DishId;
END
GO

-- Delete Dish
CREATE OR ALTER PROCEDURE sp_DeleteDish
@DishId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- First delete related records
        DELETE FROM DishAllergens WHERE DishID = @DishId;
        DELETE FROM DishImages WHERE DishID = @DishId;
        DELETE FROM MenuDishes WHERE DishID = @DishId;

        -- Then delete the dish
        DELETE FROM Dishes WHERE DishID = @DishId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

-- Search Dishes
CREATE OR ALTER PROCEDURE sp_SearchDishes
    @Keyword NVARCHAR(100) = NULL,
    @AllergenId INT = NULL,
    @ExcludeAllergen BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Search by keyword only
    IF @Keyword IS NOT NULL AND @AllergenId IS NULL
        BEGIN
            SELECT DISTINCT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
                            d.Price, d.TotalQuantity
            FROM Dishes d
            WHERE d.DishName LIKE '%' + @Keyword + '%'
            ORDER BY d.DishName;
            RETURN;
        END

    -- Filter by allergen only
    IF @Keyword IS NULL AND @AllergenId IS NOT NULL
        BEGIN
            IF @ExcludeAllergen = 0
                BEGIN
                    -- Dishes WITH specified allergen
                    SELECT DISTINCT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
                                    d.Price, d.TotalQuantity
                    FROM Dishes d
                             INNER JOIN DishAllergens da ON d.DishID = da.DishID
                    WHERE da.AllergenID = @AllergenId
                    ORDER BY d.DishName;
                END
            ELSE
                BEGIN
                    -- Dishes WITHOUT specified allergen
                    SELECT DISTINCT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
                                    d.Price, d.TotalQuantity
                    FROM Dishes d
                    WHERE d.DishID NOT IN (
                        SELECT DishID
                        FROM DishAllergens
                        WHERE AllergenID = @AllergenId
                    )
                    ORDER BY d.DishName;
                END
            RETURN;
        END

    -- Search by both keyword and allergen
    IF @Keyword IS NOT NULL AND @AllergenId IS NOT NULL
        BEGIN
            IF @ExcludeAllergen = 0
                BEGIN
                    -- Dishes WITH specified allergen matching keyword
                    SELECT DISTINCT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
                                    d.Price, d.TotalQuantity
                    FROM Dishes d
                             INNER JOIN DishAllergens da ON d.DishID = da.DishID
                    WHERE da.AllergenID = @AllergenId
                      AND d.DishName LIKE '%' + @Keyword + '%'
                    ORDER BY d.DishName;
                END
            ELSE
                BEGIN
                    -- Dishes WITHOUT specified allergen matching keyword
                    SELECT DISTINCT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
                                    d.Price, d.TotalQuantity
                    FROM Dishes d
                    WHERE d.DishID NOT IN (
                        SELECT DishID
                        FROM DishAllergens
                        WHERE AllergenID = @AllergenId
                    )
                      AND d.DishName LIKE '%' + @Keyword + '%'
                    ORDER BY d.DishName;
                END
            RETURN;
        END

    -- If no parameters specified, return all dishes
    SELECT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
           d.Price, d.TotalQuantity
    FROM Dishes d
    ORDER BY d.DishName;
END
GO

-- Get Low Stock Dishes
CREATE OR ALTER PROCEDURE sp_GetLowStockDishes
@Threshold DECIMAL(10, 2)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
           d.Price, d.TotalQuantity,
           c.CategoryName
    FROM Dishes d
             JOIN Categories c ON d.CategoryID = c.CategoryID
    WHERE d.TotalQuantity <= @Threshold
    ORDER BY d.TotalQuantity;
END
GO

-- Get Dishes By Allergen
CREATE OR ALTER PROCEDURE sp_GetDishesByAllergen
@AllergenId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT d.DishID, d.CategoryID, d.DishName, d.PortionQuantity,
                    d.Price, d.TotalQuantity,
                    c.CategoryName
    FROM Dishes d
             INNER JOIN DishAllergens da ON d.DishID = da.DishID
             LEFT JOIN Categories c ON d.CategoryID = c.CategoryID
    WHERE da.AllergenID = @AllergenId
    ORDER BY d.DishName;
END
GO

-- Get Dishes For Menu
CREATE OR ALTER PROCEDURE sp_GetDishesForMenu
@MenuId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT md.DishID, md.MenuID, md.DishID, md.DishQuantity,
           d.DishName, d.PortionQuantity, d.Price, d.TotalQuantity
    FROM MenuDishes md
             INNER JOIN Dishes d ON md.DishID = d.DishID
    WHERE md.MenuID = @MenuId
    ORDER BY d.DishName;
END
GO

-- Add Dish To Menu
CREATE OR ALTER PROCEDURE sp_AddDishToMenu
    @MenuId INT,
    @DishId INT,
    @CustomQuantity INT,
    @MenuDishId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the dish is already in the menu
    IF EXISTS (SELECT 1 FROM MenuDishes WHERE MenuID = @MenuId AND DishID = @DishId)
        BEGIN
            -- Update the custom quantity instead
            UPDATE MenuDishes
            SET DishQuantity = @CustomQuantity
            WHERE MenuID = @MenuId AND DishID = @DishId;

            SELECT @DishId = @DishID
            FROM MenuDishes
            WHERE MenuID = @MenuId AND DishID = @DishId;
        END
    ELSE
        BEGIN
            -- Add the dish to the menu
            INSERT INTO MenuDishes (MenuID, DishID, DishQuantity)
            VALUES (@MenuId, @DishId, @CustomQuantity);

            SET @MenuDishId = SCOPE_IDENTITY();
        END
END
GO

-- Remove Dish From Menu
CREATE OR ALTER PROCEDURE sp_RemoveDishFromMenu
@MenuDishId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM MenuDishes
    WHERE DishID = @MenuDishId;
END
GO

-- Get Images For Dish
CREATE OR ALTER PROCEDURE sp_GetImagesForDish
@DishId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ImageID, DishID, ImagePath
    FROM DishImages
    WHERE DishID = @DishId
    ORDER BY ImageID;
END
GO

-- Add Dish Image
CREATE OR ALTER PROCEDURE sp_AddDishImage
    @DishId INT,
    @ImagePath NVARCHAR(255),
    @ImageId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DishImages (DishID, ImagePath)
    VALUES (@DishId, @ImagePath);

    SET @ImageId = SCOPE_IDENTITY();
END
GO

-- Delete Dish Image
CREATE OR ALTER PROCEDURE sp_DeleteDishImage
@ImageId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM DishImages WHERE ImageID = @ImageId;
END
GO

-- Update Dish Quantity
CREATE OR ALTER PROCEDURE sp_UpdateDishQuantity
    @DishId INT,
    @QuantityUsed DECIMAL(10, 2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Dishes
    SET TotalQuantity = TotalQuantity - @QuantityUsed
    WHERE DishID = @DishId;

    SELECT @@ROWCOUNT;
END
GO