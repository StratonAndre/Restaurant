-- CORRECTED MENU PROCEDURES
-- Fixed to match actual database schema without Description and DiscountPercentage columns

-- Get All Menus
CREATE OR ALTER PROCEDURE sp_GetAllMenus
AS
BEGIN
    SET NOCOUNT ON;

    SELECT m.MenuID, m.CategoryID, m.MenuName,
           c.CategoryName
    FROM Menus m
             LEFT JOIN Categories c ON m.CategoryID = c.CategoryID
    ORDER BY m.MenuName;
END
GO

-- Get Menu Details
CREATE OR ALTER PROCEDURE sp_GetMenuDetails
@MenuId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT m.MenuID, m.CategoryID, m.MenuName,
           c.CategoryName
    FROM Menus m
             LEFT JOIN Categories c ON m.CategoryID = c.CategoryID
    WHERE m.MenuID = @MenuId;
END
GO

-- Get Menus By Category
CREATE OR ALTER PROCEDURE sp_GetMenusByCategory
@CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT m.MenuID, m.CategoryID, m.MenuName
    FROM Menus m
    WHERE m.CategoryID = @CategoryId
    ORDER BY m.MenuName;
END
GO

-- Insert Menu
CREATE OR ALTER PROCEDURE sp_InsertMenu
    @CategoryId INT,
    @Name NVARCHAR(100),
    @MenuId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Menus (CategoryID, MenuName)
    VALUES (@CategoryId, @Name);

    SET @MenuId = SCOPE_IDENTITY();
END
GO

-- Update Menu
CREATE OR ALTER PROCEDURE sp_UpdateMenu
    @MenuId INT,
    @CategoryId INT,
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Menus
    SET CategoryID = @CategoryId,
        MenuName = @Name
    WHERE MenuID = @MenuId;
END
GO

-- Delete Menu
CREATE OR ALTER PROCEDURE sp_DeleteMenu
@MenuId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- First delete the menu dishes
        DELETE FROM MenuDishes WHERE MenuID = @MenuId;

        -- Then delete the menu
        DELETE FROM Menus WHERE MenuID = @MenuId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

-- Get Dishes For Menu
CREATE OR ALTER PROCEDURE sp_GetDishesForMenu
@MenuId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT md.DishID, md.MenuID, md.DishID, md.DishQuantity AS CustomQuantity,
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

            SELECT @MenuDishId = DishID  -- Using DishID since MenuDishID doesn't exist
            FROM MenuDishes
            WHERE MenuID = @MenuId AND DishID = @DishId;
        END
    ELSE
        BEGIN
            -- Add the dish to the menu
            INSERT INTO MenuDishes (MenuID, DishID, DishQuantity)
            VALUES (@MenuId, @DishId, @CustomQuantity);

            SET @MenuDishId = @DishId;  -- Return DishID since MenuDishID doesn't exist
        END
END
GO

-- Remove Dish From Menu
CREATE OR ALTER PROCEDURE sp_RemoveDishFromMenu
    @MenuId INT,
    @DishId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM MenuDishes
    WHERE MenuID = @MenuId AND DishID = @DishId;
END
GO

-- Search Menus
CREATE OR ALTER PROCEDURE sp_SearchMenus
    @Keyword NVARCHAR(100) = NULL,
    @AllergenId INT = NULL,
    @ExcludeAllergen BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Search by keyword only
    IF @Keyword IS NOT NULL AND @AllergenId IS NULL
        BEGIN
            SELECT DISTINCT m.MenuID, m.CategoryID, m.MenuName
            FROM Menus m
            WHERE m.MenuName LIKE '%' + @Keyword + '%'
            ORDER BY m.MenuName;
            RETURN;
        END

    -- Filter by allergen only
    IF @Keyword IS NULL AND @AllergenId IS NOT NULL
        BEGIN
            IF @ExcludeAllergen = 0
                BEGIN
                    -- Menus WITH dishes that have the specified allergen
                    SELECT DISTINCT m.MenuID, m.CategoryID, m.MenuName
                    FROM Menus m
                             INNER JOIN MenuDishes md ON m.MenuID = md.MenuID
                             INNER JOIN Dishes d ON md.DishID = d.DishID
                             INNER JOIN DishAllergens da ON d.DishID = da.DishID
                    WHERE da.AllergenID = @AllergenId
                    ORDER BY m.MenuName;
                END
            ELSE
                BEGIN
                    -- Menus WITHOUT dishes that have the specified allergen
                    SELECT DISTINCT m.MenuID, m.CategoryID, m.MenuName
                    FROM Menus m
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM MenuDishes md
                                 INNER JOIN Dishes d ON md.DishID = d.DishID
                                 INNER JOIN DishAllergens da ON d.DishID = da.DishID
                        WHERE md.MenuID = m.MenuID
                          AND da.AllergenID = @AllergenId
                    )
                    ORDER BY m.MenuName;
                END
            RETURN;
        END

    -- Search by both keyword and allergen
    IF @Keyword IS NOT NULL AND @AllergenId IS NOT NULL
        BEGIN
            IF @ExcludeAllergen = 0
                BEGIN
                    -- Menus WITH dishes that have the specified allergen and match keyword
                    SELECT DISTINCT m.MenuID, m.CategoryID, m.MenuName
                    FROM Menus m
                             INNER JOIN MenuDishes md ON m.MenuID = md.MenuID
                             INNER JOIN Dishes d ON md.DishID = d.DishID
                             INNER JOIN DishAllergens da ON d.DishID = da.DishID
                    WHERE da.AllergenID = @AllergenId
                      AND m.MenuName LIKE '%' + @Keyword + '%'
                    ORDER BY m.MenuName;
                END
            ELSE
                BEGIN
                    -- Menus WITHOUT dishes that have the specified allergen and match keyword
                    SELECT DISTINCT m.MenuID, m.CategoryID, m.MenuName
                    FROM Menus m
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM MenuDishes md
                                 INNER JOIN Dishes d ON md.DishID = d.DishID
                                 INNER JOIN DishAllergens da ON d.DishID = da.DishID
                        WHERE md.MenuID = m.MenuID
                          AND da.AllergenID = @AllergenId
                    )
                      AND m.MenuName LIKE '%' + @Keyword + '%'
                    ORDER BY m.MenuName;
                END
            RETURN;
        END

    -- If no parameters specified, return all menus
    SELECT m.MenuID, m.CategoryID, m.MenuName
    FROM Menus m
    ORDER BY m.MenuName;
END
GO