-- CORRECTED ALLERGEN PROCEDURES
-- Fixed to match actual database schema

-- Get All Allergens
CREATE OR ALTER PROCEDURE sp_GetAllAllergens
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AllergenID, AllergenName
    FROM Allergens
    ORDER BY AllergenName;
END
GO

-- Get Allergen By ID
CREATE OR ALTER PROCEDURE sp_GetAllergenById
@AllergenId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AllergenID, AllergenName
    FROM Allergens
    WHERE AllergenID = @AllergenId;
END
GO

-- Insert Allergen
CREATE OR ALTER PROCEDURE sp_InsertAllergen
    @Name NVARCHAR(100),
    @AllergenId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Allergens (AllergenName)
    VALUES (@Name);

    SET @AllergenId = SCOPE_IDENTITY();
END
GO

-- Update Allergen
CREATE OR ALTER PROCEDURE sp_UpdateAllergen
    @AllergenId INT,
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Allergens
    SET AllergenName = @Name
    WHERE AllergenID = @AllergenId;
END
GO

-- Delete Allergen
CREATE OR ALTER PROCEDURE sp_DeleteAllergen
@AllergenId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- First delete the associations
    DELETE FROM DishAllergens WHERE AllergenID = @AllergenId;

    -- Then delete the allergen
    DELETE FROM Allergens WHERE AllergenID = @AllergenId;
END
GO

-- Get Allergens For Dish
CREATE OR ALTER PROCEDURE sp_GetAllergensForDish
@DishId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.AllergenID, a.AllergenName
    FROM Allergens a
             INNER JOIN DishAllergens da ON a.AllergenID = da.AllergenID
    WHERE da.DishID = @DishId
    ORDER BY a.AllergenName;
END
GO

-- Add Allergen To Dish
CREATE OR ALTER PROCEDURE sp_AddAllergenToDish
    @DishId INT,
    @AllergenId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the association already exists
    IF EXISTS (SELECT 1 FROM DishAllergens WHERE DishID = @DishId AND AllergenID = @AllergenId)
        BEGIN
            RETURN; -- Already exists, nothing to do
        END

    -- Add the association
    INSERT INTO DishAllergens (DishID, AllergenID)
    VALUES (@DishId, @AllergenId);
END
GO

-- Remove Allergen From Dish
CREATE OR ALTER PROCEDURE sp_RemoveAllergenFromDish
    @DishId INT,
    @AllergenId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM DishAllergens
    WHERE DishID = @DishId AND AllergenID = @AllergenId;
END
GO