using RestaurantManager.Models;
using RestaurantManager.Services;
using RestaurantManager.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestaurantManager.ViewModels
{
    /// <summary>
    /// ViewModel for the customer profile.
    /// </summary>
    public class CustomerViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _deliveryAddress;
        private string _currentPassword;
        private string _newPassword;
        private string _confirmPassword;
        private bool _isEditingProfile;
        private bool _isChangingPassword;
        private bool _isLoadingData;
        
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }
        
        /// <summary>
        /// Gets or sets the delivery address.
        /// </summary>
        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set => SetProperty(ref _deliveryAddress, value);
        }
        
        /// <summary>
        /// Gets or sets the current password.
        /// </summary>
        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }
        
        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }
        
        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the customer is editing their profile.
        /// </summary>
        public bool IsEditingProfile
        {
            get => _isEditingProfile;
            set => SetProperty(ref _isEditingProfile, value);
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the customer is changing their password.
        /// </summary>
        public bool IsChangingPassword
        {
            get => _isChangingPassword;
            set => SetProperty(ref _isChangingPassword, value);
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether data is being loaded.
        /// </summary>
        public bool IsLoadingData
        {
            get => _isLoadingData;
            set => SetProperty(ref _isLoadingData, value);
        }
        
        /// <summary>
        /// Gets the current user.
        /// </summary>
        public User CurrentUser => _userService.CurrentUser;
        
        /// <summary>
        /// Gets a value indicating whether the user is authenticated.
        /// </summary>
        public bool IsUserAuthenticated => _userService.IsAuthenticated;
        
        /// <summary>
        /// Gets a value indicating whether the user is a client.
        /// </summary>
        public bool IsClient => _userService.IsClient;
        
        /// <summary>
        /// Gets the command to edit the profile.
        /// </summary>
        public ICommand EditProfileCommand { get; }
        
        /// <summary>
        /// Gets the command to save the profile.
        /// </summary>
        public ICommand SaveProfileCommand { get; }
        
        /// <summary>
        /// Gets the command to cancel editing the profile.
        /// </summary>
        public ICommand CancelEditProfileCommand { get; }
        
        /// <summary>
        /// Gets the command to change the password.
        /// </summary>
        public ICommand ChangePasswordCommand { get; }
        
        /// <summary>
        /// Gets the command to save the password.
        /// </summary>
        public ICommand SavePasswordCommand { get; }
        
        /// <summary>
        /// Gets the command to cancel changing the password.
        /// </summary>
        public ICommand CancelChangePasswordCommand { get; }
        
        /// <summary>
        /// Gets the command to view orders.
        /// </summary>
        public ICommand ViewOrdersCommand { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerViewModel"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public CustomerViewModel(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            
            Title = "My Profile";
            
            EditProfileCommand = new RelayCommand(EditProfile, () => IsUserAuthenticated && !IsEditingProfile && !IsChangingPassword);
            SaveProfileCommand = new RelayCommand(SaveProfile, CanSaveProfile);
            CancelEditProfileCommand = new RelayCommand(CancelEditProfile, () => IsEditingProfile);
            
            ChangePasswordCommand = new RelayCommand(ChangePassword, () => IsUserAuthenticated && !IsEditingProfile && !IsChangingPassword);
            SavePasswordCommand = new RelayCommand(SavePassword, CanSavePassword);
            CancelChangePasswordCommand = new RelayCommand(CancelChangePassword, () => IsChangingPassword);
            
            ViewOrdersCommand = new RelayCommand(ViewOrders, () => IsUserAuthenticated);
            
            // Load user data
            LoadUserDataAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Loads the user data.
        /// </summary>
        private async Task LoadUserDataAsync()
        {
            if (!IsUserAuthenticated)
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Load the user's profile
                var user = await _userService.GetUserDetailsAsync(CurrentUser.UserId);
                
                if (user != null)
                {
                    // Update the properties
                    FirstName = user.FirstName;
                    LastName = user.LastName;
                    Email = user.Email;
                    PhoneNumber = user.PhoneNumber;
                    DeliveryAddress = user.DeliveryAddress;
                }
                else
                {
                    ErrorMessage = "Failed to load user profile.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load user profile: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Edits the profile.
        /// </summary>
        private void EditProfile()
        {
            IsEditingProfile = true;
        }
        
        /// <summary>
        /// Determines whether the profile can be saved.
        /// </summary>
        /// <returns>True if the profile can be saved, false otherwise.</returns>
        private bool CanSaveProfile()
        {
            return IsEditingProfile && 
                   !string.IsNullOrWhiteSpace(FirstName) && 
                   !string.IsNullOrWhiteSpace(LastName) && 
                   !string.IsNullOrWhiteSpace(PhoneNumber) && 
                   !string.IsNullOrWhiteSpace(DeliveryAddress);
        }
        
        /// <summary>
        /// Saves the profile.
        /// </summary>
        private async void SaveProfile()
        {
            if (!CanSaveProfile())
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                // Update user
                var user = new User
                {
                    UserId = CurrentUser.UserId,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email, // Email cannot be changed
                    PhoneNumber = PhoneNumber,
                    DeliveryAddress = DeliveryAddress,
                    RoleId = CurrentUser.RoleId
                };
                
                bool success = await _userService.UpdateUserAsync(user);
                
                if (success)
                {
                    IsEditingProfile = false;
                    MessageBox.Show("Your profile has been updated successfully.", "Profile Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ErrorMessage = "Failed to update profile.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update profile: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Cancels editing the profile.
        /// </summary>
        private void CancelEditProfile()
        {
            IsEditingProfile = false;
            
            // Reload the user data
            LoadUserDataAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Changes the password.
        /// </summary>
        private void ChangePassword()
        {
            IsChangingPassword = true;
            
            // Clear password fields
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }
        
        /// <summary>
        /// Determines whether the password can be saved.
        /// </summary>
        /// <returns>True if the password can be saved, false otherwise.</returns>
        private bool CanSavePassword()
        {
            return IsChangingPassword && 
                   !string.IsNullOrWhiteSpace(CurrentPassword) && 
                   !string.IsNullOrWhiteSpace(NewPassword) && 
                   !string.IsNullOrWhiteSpace(ConfirmPassword) && 
                   NewPassword == ConfirmPassword;
        }
        
        /// <summary>
        /// Saves the password.
        /// </summary>
        private async void SavePassword()
        {
            if (!CanSavePassword())
            {
                return;
            }
            
            try
            {
                IsLoadingData = true;
                ErrorMessage = null;
                
                bool success = await _userService.ChangePasswordAsync(CurrentUser.UserId, CurrentPassword, NewPassword);
                
                if (success)
                {
                    IsChangingPassword = false;
                    MessageBox.Show("Your password has been changed successfully.", "Password Changed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ErrorMessage = "Failed to change password. Please check your current password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to change password: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        
        /// <summary>
        /// Cancels changing the password.
        /// </summary>
        private void CancelChangePassword()
        {
            IsChangingPassword = false;
            
            // Clear password fields
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }
        
        /// <summary>
        /// Views the orders.
        /// </summary>
        private void ViewOrders()
        {
            // This will be implemented by the parent view model
            // or navigation service
        }
    }
}