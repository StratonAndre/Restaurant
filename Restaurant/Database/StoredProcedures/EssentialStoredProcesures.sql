USE Restaurant;
GO

-- User Authentication
CREATE OR ALTER PROCEDURE sp_SafeAuthenticateUser
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(128)
    AS
BEGIN
    SET NOCOUNT ON;

SELECT u.*, r.RoleName
FROM Users u
         JOIN UserRoles r ON u.RoleId = r.RoleId
WHERE u.Email = @Email AND u.PasswordHash = @PasswordHash;
END
GO

-- Get All Categories
CREATE OR ALTER PROCEDURE sp_GetAllCategories
    AS
BEGIN
    SET NOCOUNT ON;

SELECT CategoryID, CategoryName
FROM Categories
ORDER BY CategoryName;
END
GO

-- Get All Settings
CREATE OR ALTER PROCEDURE sp_GetAllSettings
    AS
BEGIN
    SET NOCOUNT ON;

SELECT SettingID, SettingName, SettingValue
FROM AppSettings
ORDER BY SettingName;
END
GO

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

PRINT 'Essential stored procedures created successfully!';
GO