-- CORRECTED CATEGORY PROCEDURES
-- Fixed to match your actual database schema (no Description column)

USE Restaurant;
GO

-- Insert Category (FIXED - removed Description parameter completely)
CREATE OR ALTER PROCEDURE sp_InsertCategory
    @Name NVARCHAR(100),
    @CategoryId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Categories (CategoryName)
    VALUES (@Name);

    SET @CategoryId = SCOPE_IDENTITY();
END
GO

-- Update Category (FIXED - removed Description parameter completely)
CREATE OR ALTER PROCEDURE sp_UpdateCategory
    @CategoryId INT,
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Categories
    SET CategoryName = @Name
    WHERE CategoryID = @CategoryId;
END
GO