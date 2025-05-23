using RestaurantManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RestaurantManager.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
            Loaded += RegisterView_Loaded;
        }

        private void RegisterView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up password binding when the view is loaded
            if (DataContext is RegisterViewModel viewModel)
            {
                SetupPasswordBinding(viewModel);
            }
            
            // Also handle DataContext changes
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is RegisterViewModel viewModel)
            {
                SetupPasswordBinding(viewModel);
            }
        }

        private void SetupPasswordBinding(RegisterViewModel viewModel)
        {
            // Set up password field binding
            PasswordBox.PasswordChanged += (s, args) =>
            {
                viewModel.Password = PasswordBox.Password;
            };

            // Set up confirm password field binding
            ConfirmPasswordBox.PasswordChanged += (s, args) =>
            {
                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            };
        }
    }
}