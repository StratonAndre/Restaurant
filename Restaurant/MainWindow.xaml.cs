using RestaurantManager.Services;
using RestaurantManager.ViewModels;
using System;
using System.Windows;

namespace RestaurantManager.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ShoppingCartViewModel _shoppingCartViewModel;
        
        // ViewModels
        private MenuViewModel? _menuViewModel;
        private SearchViewModel? _searchViewModel;
        private LoginViewModel? _loginViewModel;
        private RegisterViewModel? _registerViewModel;
        private CustomerOrdersViewModel? _customerOrdersViewModel;
        private OrderManagementViewModel? _orderManagementViewModel;
        private DishManagementViewModel? _dishManagementViewModel;
        private MenuManagementViewModel? _menuManagementViewModel;
        private CategoryManagementViewModel? _categoryManagementViewModel;
        private AllergenManagementViewModel? _allergenManagementViewModel;
        private AdminViewModel? _adminViewModel;
        private CustomerViewModel? _customerViewModel;
        private ShoppingCartView? _shoppingCartView;
        
        // Services
        private readonly UserService _userService;
        private readonly CategoryService _categoryService;
        private readonly AllergenService _allergenService;
        private readonly DishService _dishService;
        private readonly MenuService _menuService;
        private readonly OrderService _orderService;
        private readonly SearchService _searchService;
        private readonly ConfigurationService _configurationService;

        public MainWindow(MainViewModel mainViewModel, ShoppingCartViewModel shoppingCartViewModel)
        {
            InitializeComponent();
            
            _mainViewModel = mainViewModel;
            _shoppingCartViewModel = shoppingCartViewModel;
            
            // Get references to services from the application
            var app = (App)Application.Current;
            _userService = app._userService;
            _categoryService = app._categoryService;
            _allergenService = app._allergenService;
            _dishService = app._dishService;
            _menuService = app._menuService;
            _orderService = app._orderService;
            _searchService = app._searchService;
            _configurationService = app._configurationService;
            
            // Set up event handlers
            _mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
            
            // Initialize the current view
            UpdateCurrentView(_mainViewModel.CurrentView);
        }

        private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.CurrentView))
            {
                UpdateCurrentView(_mainViewModel.CurrentView);
            }
        }

        private void UpdateCurrentView(AppView view)
        {
            try
            {
                switch (view)
                {
                    case AppView.Menu:
                        if (_menuViewModel == null)
                        {
                            _menuViewModel = new MenuViewModel(_categoryService, _dishService, _menuService, _userService);
                        }
                        _mainViewModel.CurrentViewModel = _menuViewModel;
                        break;
                        
                    case AppView.Search:
                        if (_searchViewModel == null)
                        {
                            _searchViewModel = new SearchViewModel(_searchService, _allergenService, _userService);
                        }
                        _mainViewModel.CurrentViewModel = _searchViewModel;
                        break;
                        
                    case AppView.Login:
                        _loginViewModel = new LoginViewModel(_userService);
                        _loginViewModel.LoginSuccessful += (s, user) => _mainViewModel.HandleUserLogin(user);
                        _loginViewModel.RegisterRequested += (s, e) => _mainViewModel.Navigate(AppView.Register);
                        _mainViewModel.CurrentViewModel = _loginViewModel;
                        break;
                        
                    case AppView.Register:
                        _registerViewModel = new RegisterViewModel(_userService);
                        _registerViewModel.RegistrationSuccessful += (s, user) => _mainViewModel.HandleUserLogin(user);
                        _registerViewModel.BackToLoginRequested += (s, e) => _mainViewModel.Navigate(AppView.Login);
                        _mainViewModel.CurrentViewModel = _registerViewModel;
                        break;
                        
                    case AppView.Cart:
                        if (_shoppingCartView == null)
                        {
                            _shoppingCartView = new ShoppingCartView();
                            _shoppingCartView.DataContext = _shoppingCartViewModel;
                        }
                        _mainViewModel.CurrentViewModel = _shoppingCartViewModel;
                        break;
                        
                    case AppView.CustomerOrders:
                        if (_customerOrdersViewModel == null)
                        {
                            _customerOrdersViewModel = new CustomerOrdersViewModel(_userService, _orderService);
                        }
                        _mainViewModel.CurrentViewModel = _customerOrdersViewModel;
                        break;
                        
                    case AppView.CustomerProfile:
                        if (_customerViewModel == null)
                        {
                            _customerViewModel = new CustomerViewModel(_userService);
                        }
                        _mainViewModel.CurrentViewModel = _customerViewModel;
                        break;
                        
                    case AppView.AdminDashboard:
                        if (_adminViewModel == null)
                        {
                            _adminViewModel = new AdminViewModel(_userService, _orderService, _dishService, 
                                new ReportService(_databaseService, _orderService, _dishService, _configurationService), 
                                _configurationService);
                        }
                        _mainViewModel.CurrentViewModel = _adminViewModel;
                        break;
                        
                    case AppView.OrderManagement:
                        if (_orderManagementViewModel == null)
                        {
                            _orderManagementViewModel = new OrderManagementViewModel(_userService, _orderService);
                        }
                        _mainViewModel.CurrentViewModel = _orderManagementViewModel;
                        break;
                        
                    case AppView.DishManagement:
                        if (_dishManagementViewModel == null)
                        {
                            _dishManagementViewModel = new DishManagementViewModel(
                                _userService, _dishService, _categoryService, _allergenService, _configurationService);
                        }
                        _mainViewModel.CurrentViewModel = _dishManagementViewModel;
                        break;
                        
                    case AppView.MenuManagement:
                        if (_menuManagementViewModel == null)
                        {
                            _menuManagementViewModel = new MenuManagementViewModel(
                                _userService, _menuService, _categoryService, _dishService, _configurationService);
                        }
                        _mainViewModel.CurrentViewModel = _menuManagementViewModel;
                        break;
                        
                    case AppView.CategoryManagement:
                        if (_categoryManagementViewModel == null)
                        {
                            _categoryManagementViewModel = new CategoryManagementViewModel(
                                _userService, _categoryService, _dishService, _menuService);
                        }
                        _mainViewModel.CurrentViewModel = _categoryManagementViewModel;
                        break;
                        
                    case AppView.AllergenManagement:
                        if (_allergenManagementViewModel == null)
                        {
                            _allergenManagementViewModel = new AllergenManagementViewModel(
                                _userService, _allergenService, _dishService);
                        }
                        _mainViewModel.CurrentViewModel = _allergenManagementViewModel;
                        break;
                        
                    default:
                        // Default to menu view
                        if (_menuViewModel == null)
                        {
                            _menuViewModel = new MenuViewModel(_categoryService, _dishService, _menuService, _userService);
                        }
                        _mainViewModel.CurrentViewModel = _menuViewModel;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DatabaseService _databaseService => ((App)Application.Current)._databaseService;
    }
}