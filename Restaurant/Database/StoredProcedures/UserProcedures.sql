-- 1. User Authentication (Safe from SQL Injection)
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

-- 2. User Registration
CREATE OR ALTER PROCEDURE sp_RegisterUser
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @DeliveryAddress NVARCHAR(200),
    @PasswordHash NVARCHAR(128),
    @RoleId INT,
    @UserId INT OUTPUT
    AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
BEGIN
        RAISERROR('Email already in use', 16, 1);
        RETURN;
END

INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, DeliveryAddress, PasswordHash, RoleId)
VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DeliveryAddress, @PasswordHash, @RoleId);

SET @UserId = SCOPE_IDENTITY();
END
GO

-- Get User Details
CREATE OR ALTER PROCEDURE sp_GetUserDetails
    @UserId INT
    AS
BEGIN
    SET NOCOUNT ON;

SELECT u.*, r.RoleName
FROM Users u
         LEFT JOIN UserRoles r ON u.RoleId = r.RoleId
WHERE u.UserId = @UserId;
END
GO

-- Update User Details
CREATE OR ALTER PROCEDURE sp_UpdateUserDetails
    @UserId INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @PhoneNumber NVARCHAR(20),
    @DeliveryAddress NVARCHAR(200)
    AS
BEGIN
    SET NOCOUNT ON;

UPDATE Users
SET FirstName = @FirstName,
    LastName = @LastName,
    PhoneNumber = @PhoneNumber,
    DeliveryAddress = @DeliveryAddress
WHERE UserId = @UserId;
END
GO

-- Verify User Password
CREATE OR ALTER PROCEDURE sp_VerifyUserPassword
    @UserId INT,
    @PasswordHash NVARCHAR(128)
    AS
BEGIN
    SET NOCOUNT ON;

SELECT COUNT(*)
FROM Users
WHERE UserId = @UserId AND PasswordHash = @PasswordHash;
END
GO

-- Update User Password
CREATE OR ALTER PROCEDURE sp_UpdateUserPassword
    @UserId INT,
    @PasswordHash NVARCHAR(128)
    AS
BEGIN
    SET NOCOUNT ON;

UPDATE Users
SET PasswordHash = @PasswordHash
WHERE UserId = @UserId;
END
GO