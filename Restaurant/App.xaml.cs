using RestaurantManager.Services;
using RestaurantManager.ViewModels;
using RestaurantManager.Views;
using System;
using System.Windows;

namespace RestaurantManager
{
    public partial class App : Application
    {
        // Make services public so MainWindow can access them
        public DatabaseService _databaseService;
        public ConfigurationService _configurationService;
        public UserService _userService;
        public CategoryService _categoryService;
        public AllergenService _allergenService;
        public DishService _dishService;
        public MenuService _menuService;
        public OrderService _orderService;
        public SearchService _searchService;
        
        private ShoppingCartViewModel _shoppingCartViewModel;
        private MainViewModel _mainViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Initialize services first
                InitializeServices();
                
                // Load configuration settings
               // _configurationService.LoadSettingsAsync().Wait(); comented out 
                
                // Initialize view models
                InitializeViewModels();
                
                // Create and show the main window
                CreateMainWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}\n\nDetails: {ex.InnerException?.Message}", 
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void InitializeServices()
        {
            _databaseService = new DatabaseService();
            _configurationService = new ConfigurationService(_databaseService);
            _userService = new UserService(_databaseService);
            _categoryService = new CategoryService(_databaseService);
            _allergenService = new AllergenService(_databaseService);
            _dishService = new DishService(_databaseService, _allergenService);
            _menuService = new MenuService(_databaseService, _dishService, _configurationService);
            _orderService = new OrderService(_databaseService, _dishService, _menuService, _configurationService);
            _searchService = new SearchService(_databaseService, _allergenService, _dishService, _menuService);
        }

        private void InitializeViewModels()
        {
            _shoppingCartViewModel = new ShoppingCartViewModel(_userService, _orderService, _configurationService);
            _mainViewModel = new MainViewModel(_userService, _shoppingCartViewModel);
        }

        private void CreateMainWindow()
        {
            MainWindow mainWindow = new MainWindow(_mainViewModel, _shoppingCartViewModel)
            {
                DataContext = _mainViewModel
            };
            
            // Set the main window and show it
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}