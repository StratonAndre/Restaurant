using RestaurantManager.Models;
using RestaurantManager.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using RestaurantManager.Utilities;

namespace RestaurantManager.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        
        private string _email;
        private string _password;
        private bool _isLoggingIn;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetProperty(ref _isLoggingIn, value);
        }

        // Add the missing CanLogin property
        public bool CanLogin => !string.IsNullOrWhiteSpace(Email) && 
                               !string.IsNullOrWhiteSpace(Password) && 
                               !IsLoggingIn;

        public ICommand LoginCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }

        // Event raised when login is successful
        public event EventHandler<User> LoginSuccessful;
        
        // Event raised when the user wants to register
        public event EventHandler RegisterRequested;

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
            
            Title = "Login";

            LoginCommand = new RelayCommand(async () => await LoginAsync(), () => CanLogin);
            RegisterCommand = new RelayCommand(OnRegisterRequested);
            
            // Subscribe to property changes to update CanLogin
            PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(Email) || 
                    e.PropertyName == nameof(Password) || 
                    e.PropertyName == nameof(IsLoggingIn))
                {
                    OnPropertyChanged(nameof(CanLogin));
                }
            };
        }

        private bool CanLoginExecute()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsLoggingIn;
        }

        private async Task LoginAsync()
        {
            if (!CanLoginExecute())
                return;

            try
            {
                IsLoggingIn = true;
                ErrorMessage = null;
                
                // Attempt to authenticate
                var user = await _userService.AuthenticateAsync(Email, Password);
                
                if (user != null)
                {
                    // Clear sensitive data
                    Password = string.Empty;
                    
                    // Notify successful login
                    LoginSuccessful?.Invoke(this, user);
                }
                else
                {
                    ErrorMessage = "Invalid email or password. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        private void OnRegisterRequested()
        {
            RegisterRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}