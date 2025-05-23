-- Insert allergens (using correct column name: AllergenName instead of Name)
-- Note: Removed Description column as it doesn't exist in your table
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

-- Create a default admin user (password: admin123 - hashed)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@restaurant.com')
    BEGIN
        INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, DeliveryAddress, PasswordHash, RoleId)
        VALUES ('Admin', 'User', 'admin@restaurant.com', '0700000000', 'Restaurant Address',
                '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', -- SHA256 of 'admin123'
                (SELECT RoleId FROM UserRoles WHERE RoleName = 'Employee'));
    END

-- Create a test client user (password: client123 - hashed)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'client@test.com')
    BEGIN
        INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, DeliveryAddress, PasswordHash, RoleId)
        VALUES ('Test', 'Client', 'client@test.com', '0700000001', 'Test Address',
                'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', -- SHA256 of 'client123'
                (SELECT RoleId FROM UserRoles WHERE RoleName = 'Client'));
    END

PRINT 'Database schema created successfully!'
GO