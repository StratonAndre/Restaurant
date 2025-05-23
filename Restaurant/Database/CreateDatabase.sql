-- CORRECTED Restaurant Database Creation Script
-- This matches all your working stored procedures

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Restaurant')
    BEGIN
        CREATE DATABASE Restaurant;
    END
GO

USE Restaurant;
GO

-- 1. UserRoles Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
    BEGIN
        CREATE TABLE UserRoles (
                                   RoleID INT IDENTITY(1,1) PRIMARY KEY,
                                   RoleName NVARCHAR(50) NOT NULL UNIQUE
        );
    END
GO

-- 2. Users Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
    BEGIN
        CREATE TABLE Users (
                               UserID INT IDENTITY(1,1) PRIMARY KEY,
                               FirstName NVARCHAR(50) NOT NULL,
                               LastName NVARCHAR(50) NOT NULL,
                               Email NVARCHAR(100) NOT NULL UNIQUE,
                               PhoneNumber NVARCHAR(20) NOT NULL,
                               DeliveryAddress NVARCHAR(200) NOT NULL,
                               PasswordHash NVARCHAR(128) NOT NULL,
                               RoleID INT NOT NULL,
                               FOREIGN KEY (RoleID) REFERENCES UserRoles(RoleID)
        );
    END
GO

-- 3. Categories Table (CORRECTED - using CategoryName)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
    BEGIN
        CREATE TABLE Categories (
                                    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
                                    CategoryName NVARCHAR(100) NOT NULL UNIQUE
        );
    END
GO

-- 4. Dishes Table (CORRECTED - using DishName, PortionQuantity, no Description, no IsAvailable)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Dishes' AND xtype='U')
    BEGIN
        CREATE TABLE Dishes (
                                DishID INT IDENTITY(1,1) PRIMARY KEY,
                                CategoryID INT NOT NULL,
                                DishName NVARCHAR(100) NOT NULL,
                                PortionQuantity INT NOT NULL, -- in grams
                                Price DECIMAL(10, 2) NOT NULL,
                                TotalQuantity DECIMAL(10, 2) NOT NULL, -- in kg
                                FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
        );
    END
GO

-- 5. Menus Table (CORRECTED - using MenuName, no Description, no DiscountPercentage)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Menus' AND xtype='U')
    BEGIN
        CREATE TABLE Menus (
                               MenuID INT IDENTITY(1,1) PRIMARY KEY,
                               CategoryID INT NOT NULL,
                               MenuName NVARCHAR(100) NOT NULL,
                               FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
        );
    END
GO

-- 6. MenuDishes Table (CORRECTED - using DishQuantity instead of CustomQuantity)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MenuDishes' AND xtype='U')
    BEGIN
        CREATE TABLE MenuDishes (
                                    MenuID INT NOT NULL,
                                    DishID INT NOT NULL,
                                    DishQuantity INT NOT NULL, -- in grams, portion size for this menu
                                    FOREIGN KEY (MenuID) REFERENCES Menus(MenuID) ON DELETE CASCADE,
                                    FOREIGN KEY (DishID) REFERENCES Dishes(DishID) ON DELETE CASCADE,
                                    UNIQUE(MenuID, DishID)
        );
    END
GO

-- 7. Allergens Table (CORRECTED - using AllergenName, no Description)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Allergens' AND xtype='U')
    BEGIN
        CREATE TABLE Allergens (
                                   AllergenID INT IDENTITY(1,1) PRIMARY KEY,
                                   AllergenName NVARCHAR(100) NOT NULL UNIQUE
        );
    END
GO

-- 8. DishAllergens Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DishAllergens' AND xtype='U')
    BEGIN
        CREATE TABLE DishAllergens (
                                       DishID INT NOT NULL,
                                       AllergenID INT NOT NULL,
                                       FOREIGN KEY (DishID) REFERENCES Dishes(DishID) ON DELETE CASCADE,
                                       FOREIGN KEY (AllergenID) REFERENCES Allergens(AllergenID) ON DELETE CASCADE,
                                       UNIQUE(DishID, AllergenID)
        );
    END
GO

-- 9. DishImages Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DishImages' AND xtype='U')
    BEGIN
        CREATE TABLE DishImages (
                                    ImageID INT IDENTITY(1,1) PRIMARY KEY,
                                    DishID INT NOT NULL,
                                    ImagePath NVARCHAR(255) NOT NULL,
                                    FOREIGN KEY (DishID) REFERENCES Dishes(DishID) ON DELETE CASCADE
        );
    END
GO

-- 10. OrderStatus Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderStatus' AND xtype='U')
    BEGIN
        CREATE TABLE OrderStatus (
                                     StatusID INT IDENTITY(1,1) PRIMARY KEY,
                                     StatusName NVARCHAR(50) NOT NULL UNIQUE
        );
    END
GO

-- 11. Orders Table (CORRECTED - with DiscountAmount)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
    BEGIN
        CREATE TABLE Orders (
                                OrderID INT IDENTITY(1,1) PRIMARY KEY,
                                OrderCode NVARCHAR(20) NOT NULL UNIQUE,
                                UserID INT NOT NULL,
                                OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
                                EstimatedDeliveryTime DATETIME NOT NULL,
                                StatusID INT NOT NULL,
                                FoodCost DECIMAL(10, 2) NOT NULL,
                                DeliveryCost DECIMAL(10, 2) NOT NULL DEFAULT 0,
                                DiscountAmount DECIMAL(10, 2) NOT NULL DEFAULT 0,
                                TotalCost AS (FoodCost + DeliveryCost - DiscountAmount) PERSISTED,
                                FOREIGN KEY (UserID) REFERENCES Users(UserID),
                                FOREIGN KEY (StatusID) REFERENCES OrderStatus(StatusID)
        );
    END
GO

-- 12. OrderDetails Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderDetails' AND xtype='U')
    BEGIN
        CREATE TABLE OrderDetails (
                                      OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
                                      OrderID INT NOT NULL,
                                      DishID INT NULL, -- Either DishID or MenuID should be filled
                                      MenuID INT NULL, -- Either DishID or MenuID should be filled
                                      Quantity INT NOT NULL,
                                      UnitPrice DECIMAL(10, 2) NOT NULL,
                                      FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
                                      FOREIGN KEY (DishID) REFERENCES Dishes(DishID),
                                      FOREIGN KEY (MenuID) REFERENCES Menus(MenuID),
                                      CHECK ((DishID IS NOT NULL AND MenuID IS NULL) OR (DishID IS NULL AND MenuID IS NOT NULL))
        );
    END
GO

-- 13. AppSettings Table (CORRECTED - using SettingName, no Description)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AppSettings' AND xtype='U')
    BEGIN
        CREATE TABLE AppSettings (
                                     SettingID INT IDENTITY(1,1) PRIMARY KEY,
                                     SettingName NVARCHAR(100) NOT NULL UNIQUE,
                                     SettingValue NVARCHAR(500) NOT NULL
        );
    END
GO

-- Insert Initial Data

-- Insert User Roles
IF NOT EXISTS (SELECT * FROM UserRoles WHERE RoleName = 'Client')
    BEGIN
        INSERT INTO UserRoles (RoleName) VALUES ('Client');
        INSERT INTO UserRoles (RoleName) VALUES ('Employee');
    END
GO

-- Insert Order Statuses
IF NOT EXISTS (SELECT * FROM OrderStatus WHERE StatusName = 'Registered')
    BEGIN
        INSERT INTO OrderStatus (StatusName) VALUES ('Registered');
        INSERT INTO OrderStatus (StatusName) VALUES ('In Preparation');
        INSERT INTO OrderStatus (StatusName) VALUES ('Out for Delivery');
        INSERT INTO OrderStatus (StatusName) VALUES ('Delivered');
        INSERT INTO OrderStatus (StatusName) VALUES ('Cancelled');
    END
GO

-- Insert Default App Settings (CORRECTED - using SettingName)
IF NOT EXISTS (SELECT * FROM AppSettings WHERE SettingName = 'MenuDiscountPercentage')
    BEGIN
        INSERT INTO AppSettings (SettingName, SettingValue) VALUES
                                                                ('MenuDiscountPercentage', '10'),
                                                                ('MinOrderForFreeDelivery', '100'),
                                                                ('StandardDeliveryCost', '15'),
                                                                ('OrderDiscountThreshold', '200'),
                                                                ('OrderDiscountPercentage', '5'),
                                                                ('FrequentOrderCount', '3'),
                                                                ('FrequentOrderPeriodDays', '30'),
                                                                ('FrequentOrderDiscount', '7'),
                                                                ('LowStockThreshold', '2');
    END
GO

-- Insert Sample Categories (CORRECTED - using CategoryName)
IF NOT EXISTS (SELECT * FROM Categories WHERE CategoryName = 'Mic Dejun')
    BEGIN
        INSERT INTO Categories (CategoryName) VALUES
                                                  ('Mic Dejun'),
                                                  ('Aperitive'),
                                                  ('Supe și Ciorbe'),
                                                  ('Fel Principal'),
                                                  ('Deserturi'),
                                                  ('Băuturi');
    END
GO

-- Insert Sample Allergens (CORRECTED - using AllergenName)
IF NOT EXISTS (SELECT * FROM Allergens WHERE AllergenName = 'Gluten')
    BEGIN
        INSERT INTO Allergens (AllergenName) VALUES
                                                 ('Gluten'),
                                                 ('Ouă'),
                                                 ('Lactoza'),
                                                 ('Nucă'),
                                                 ('Soia'),
                                                 ('Pește'),
                                                 ('Țelină');
    END
GO

-- Create a default admin user (password: admin123 - hashed)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@restaurant.com')
    BEGIN
        INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, DeliveryAddress, PasswordHash, RoleID)
        VALUES ('Admin', 'User', 'admin@restaurant.com', '0700000000', 'Restaurant Address',
                '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', -- SHA256 of 'admin123'
                (SELECT RoleID FROM UserRoles WHERE RoleName = 'Employee'));
    END
GO

-- Create a test client user (password: client123 - hashed)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'client@test.com')
    BEGIN
        INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, DeliveryAddress, PasswordHash, RoleID)
        VALUES ('Test', 'Client', 'client@test.com', '0700000001', 'Test Address',
                'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', -- SHA256 of 'client123'
                (SELECT RoleID FROM UserRoles WHERE RoleName = 'Client'));
    END
GO

PRINT 'Database created successfully with corrected schema!';
PRINT 'Schema matches all your working stored procedures.';
GO