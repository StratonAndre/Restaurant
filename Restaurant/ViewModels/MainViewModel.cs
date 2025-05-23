using RestaurantManager.Models;
using RestaurantManager.Services;
using System;
using System.Windows.Input;
using RestaurantManager.Utilities;
namespace RestaurantManager.ViewModels
{
    public enum AppView
    {
        Menu,
        Search,
        Login,
        Register,
        Cart,
        CustomerOrders,
        CustomerProfile,
        AdminDashboard,
        CategoryManagement,
        DishManagement,
        MenuManagement,
        AllergenManagement,
        OrderManagement
    }

    public class MainViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly ShoppingCartViewModel _cartViewModel;
        
        private BaseViewModel _currentViewModel;
        private AppView _currentView;
        private bool _isMenuVisible;
        private bool _isUserAuthenticated;
        private bool _isClient;
        private bool _isEmployee;
        private int _cartItemCount;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public AppView CurrentView
        {
            get => _currentView;
            set
            {
                if (SetProperty(ref _currentView, value))
                {
                    OnCurrentViewChanged();
                }
            }
        }

        public bool IsMenuVisible
        {
            get => _isMenuVisible;
            set => SetProperty(ref _isMenuVisible, value);
        }

        public bool IsUserAuthenticated
        {
            get => _isUserAuthenticated;
            set => SetProperty(ref _isUserAuthenticated, value);
        }

        public bool IsClient
        {
            get => _isClient;
            set => SetProperty(ref _isClient, value);
        }

        public bool IsEmployee
        {
            get => _isEmployee;
            set => SetProperty(ref _isEmployee, value);
        }

        public int CartItemCount
        {
            get => _cartItemCount;
            set => SetProperty(ref _cartItemCount, value);
        }

        public User CurrentUser => _userService.CurrentUser;

        public ICommand NavigateCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        public MainViewModel(UserService userService, ShoppingCartViewModel cartViewModel)
        {
            _userService = userService;
            _cartViewModel = cartViewModel;
            
            Title = "Restaurant Manager";
            IsMenuVisible = true;

            NavigateCommand = new RelayCommand<AppView>(Navigate);
            LogoutCommand = new RelayCommand(Logout);
            
            _cartViewModel.CartItems.CollectionChanged += (s, e) => UpdateCartItemCount();
            
            // Set initial view
            CurrentView = AppView.Menu;
            
            // Update authentication status
            UpdateAuthenticationStatus();
        }

        // Navigate to a specific view
        public void Navigate(AppView view)
        {
            CurrentView = view;
        }

        // Handle the current view change
        private void OnCurrentViewChanged()
        {
            // Update the current view model based on the current view
            // This will be implemented by the MainWindow to switch views
        }

        // Log out the current user
        private void Logout()
        {
            _userService.Logout();
            UpdateAuthenticationStatus();
            
            // Clear the shopping cart
            _cartViewModel.ClearCart();
            
            // Navigate to the menu view
            CurrentView = AppView.Menu;
        }

        // Update the authentication status based on the current user
        private void UpdateAuthenticationStatus()
        {
            IsUserAuthenticated = _userService.IsAuthenticated;
            IsClient = _userService.IsClient;
            IsEmployee = _userService.IsEmployee;
            
            OnPropertyChanged(nameof(CurrentUser));
        }

        // Update the cart item count
        private void UpdateCartItemCount()
        {
            CartItemCount = _cartViewModel.CartItems.Count;
        }

        // Handle user login
        public void HandleUserLogin(User user)
        {
            UpdateAuthenticationStatus();
            
            // Navigate to the appropriate view based on the user's role
            if (IsClient)
            {
                CurrentView = AppView.Menu;
            }
            else if (IsEmployee)
            {
                CurrentView = AppView.AdminDashboard;
            }
        }
    }
}