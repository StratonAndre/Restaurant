using RestaurantManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public class UserService
    {
        private readonly DatabaseService _databaseService;
        private User _currentUser;

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public bool IsClient => _currentUser?.IsClient ?? false;
        public bool IsEmployee => _currentUser?.IsEmployee ?? false;

        public UserService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Authenticate a user with email and password
        public async Task<User> AuthenticateAsync(string email, string password)
        {
            try
            {
                // Hash the password for secure comparison
                string hashedPassword = HashPassword(password);

                // Use the safe stored procedure for authentication to avoid SQL injection
                var parameters = new Dictionary<string, object>
                {
                    { "Email", email },
                    { "PasswordHash", hashedPassword }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_SafeAuthenticateUser", parameters);

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    _currentUser = MapUserFromDataRow(result.Tables[0].Rows[0]);
                    return _currentUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Authentication error: {ex.Message}");
                return null;
            }
        }

        // Register a new user
        public async Task<User> RegisterUserAsync(User user, string password)
        {
            try
            {
                // Hash the password for secure storage
                user.PasswordHash = HashPassword(password);

                var parameters = new Dictionary<string, object>
                {
                    { "FirstName", user.FirstName },
                    { "LastName", user.LastName },
                    { "Email", user.Email },
                    { "PhoneNumber", user.PhoneNumber },
                    { "DeliveryAddress", user.DeliveryAddress },
                    { "PasswordHash", user.PasswordHash },
                    { "RoleId", (int)UserRoleType.Client } // New users are always clients
                };

                // Add output parameter for the new user ID
                var outputParameters = new Dictionary<string, SqlDbType>
                {
                    { "UserId", SqlDbType.Int }
                };

                var result = await _databaseService.ExecuteWithOutputAsync("sp_RegisterUser", parameters, outputParameters);

                if (result != null && result.ContainsKey("UserId"))
                {
                    user.UserId = Convert.ToInt32(result["UserId"]);
                    _currentUser = user; // Automatically log in the new user
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Registration error: {ex.Message}");
                return null;
            }
        }

        // Update user details
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "UserId", user.UserId },
                    { "FirstName", user.FirstName },
                    { "LastName", user.LastName },
                    { "PhoneNumber", user.PhoneNumber },
                    { "DeliveryAddress", user.DeliveryAddress }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateUserDetails", parameters);

                if (rowsAffected > 0)
                {
                    // Update the current user if it's the same user
                    if (_currentUser != null && _currentUser.UserId == user.UserId)
                    {
                        _currentUser.FirstName = user.FirstName;
                        _currentUser.LastName = user.LastName;
                        _currentUser.PhoneNumber = user.PhoneNumber;
                        _currentUser.DeliveryAddress = user.DeliveryAddress;
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Update user error: {ex.Message}");
                return false;
            }
        }

        // Change user password
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                // Verify current password
                string currentHashedPassword = HashPassword(currentPassword);
                
                var verifyParameters = new Dictionary<string, object>
                {
                    { "UserId", userId },
                    { "PasswordHash", currentHashedPassword }
                };

                var verifyResult = await _databaseService.ExecuteScalarAsync("sp_VerifyUserPassword", verifyParameters);
                
                if (verifyResult == null || Convert.ToInt32(verifyResult) == 0)
                {
                    return false; // Current password is incorrect
                }

                // Update to new password
                string newHashedPassword = HashPassword(newPassword);
                
                var updateParameters = new Dictionary<string, object>
                {
                    { "UserId", userId },
                    { "PasswordHash", newHashedPassword }
                };

                int rowsAffected = await _databaseService.ExecuteNonQueryAsync("sp_UpdateUserPassword", updateParameters);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Change password error: {ex.Message}");
                return false;
            }
        }

        // Log out the current user
        public void Logout()
        {
            _currentUser = null;
        }

        // Get user details by ID
        public async Task<User> GetUserDetailsAsync(int userId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "UserId", userId }
                };

                DataSet result = await _databaseService.ExecuteDataSetAsync("sp_GetUserDetails", parameters);

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    return MapUserFromDataRow(result.Tables[0].Rows[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Get user details error: {ex.Message}");
                return null;
            }
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        // Helper method to map a DataRow to a User object
        private User MapUserFromDataRow(DataRow row)
        {
            return new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["Email"].ToString(),
                PhoneNumber = row["PhoneNumber"].ToString(),
                DeliveryAddress = row["DeliveryAddress"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                RoleId = Convert.ToInt32(row["RoleId"]),
                Role = new UserRole
                {
                    RoleId = Convert.ToInt32(row["RoleId"]),
                    RoleName = row["RoleName"].ToString()
                }
            };
        }
    }
}