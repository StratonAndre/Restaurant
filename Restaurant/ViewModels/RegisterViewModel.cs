using RestaurantManager.Models;
using RestaurantManager.Services;
using RestaurantManager.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantManager.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _deliveryAddress;
        private string _password;
        private string _confirmPassword;
        private bool _isRegistering;

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
        /// Gets or sets the email address.
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
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
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
        /// Gets or sets a value indicating whether registration is in progress.
        /// </summary>
        public bool IsRegistering
        {
            get => _isRegistering;
            set => SetProperty(ref _isRegistering, value);
        }

        /// <summary>
        /// Gets the command to register a new user.
        /// </summary>
        public ICommand RegisterCommand { get; }

        /// <summary>
        /// Gets the command to go back to login.
        /// </summary>
        public ICommand BackToLoginCommand { get; }

        /// <summary>
        /// Event raised when registration is successful.
        /// </summary>
        public event EventHandler<User> RegistrationSuccessful;

        /// <summary>
        /// Event raised when user wants to go back to login.
        /// </summary>
        public event EventHandler BackToLoginRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterViewModel"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public RegisterViewModel(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            
            Title = "Register";
            
            RegisterCommand = new RelayCommand(async () => await RegisterAsync(), CanRegister);
            BackToLoginCommand = new RelayCommand(OnBackToLoginRequested);
        }

        /// <summary>
        /// Determines whether registration can be performed.
        /// </summary>
        /// <returns>True if registration can be performed, false otherwise.</returns>
        private bool CanRegister()
        {
            return !IsRegistering &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber) &&
                   !string.IsNullOrWhiteSpace(DeliveryAddress) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   Password == ConfirmPassword;
        }

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        private async Task RegisterAsync()
        {
            if (!CanRegister())
            {
                // Provide more specific error messages
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                {
                    ErrorMessage = "Please enter your full name.";
                }
                else if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Please enter your email address.";
                }
                else if (string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    ErrorMessage = "Please enter your phone number.";
                }
                else if (string.IsNullOrWhiteSpace(DeliveryAddress))
                {
                    ErrorMessage = "Please enter your delivery address.";
                }
                else if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ErrorMessage = "Please enter and confirm your password.";
                }
                else if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                }
                
                return;
            }

            try
            {
                IsRegistering = true;
                ErrorMessage = null;
                
                // Create a new user
                var user = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    DeliveryAddress = DeliveryAddress,
                    RoleId = (int)UserRoleType.Client // New users are always clients
                };
                
                // Register the user
                var registeredUser = await _userService.RegisterUserAsync(user, Password);
                
                if (registeredUser != null)
                {
                    // Clear sensitive data
                    Password = string.Empty;
                    ConfirmPassword = string.Empty;
                    
                    // Notify successful registration
                    RegistrationSuccessful?.Invoke(this, registeredUser);
                }
                else
                {
                    ErrorMessage = "Registration failed. The email may already be in use.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
            }
            finally
            {
                IsRegistering = false;
            }
        }

        /// <summary>
        /// Handles the back to login request.
        /// </summary>
        private void OnBackToLoginRequested()
        {
            BackToLoginRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}