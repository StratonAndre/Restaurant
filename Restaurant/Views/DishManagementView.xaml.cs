using Microsoft.Win32;
using RestaurantManager.ViewModels;
using RestaurantManager.Utilities; // Add this using statement for RelayCommand
using System.Windows;
using System.Windows.Controls;

namespace RestaurantManager.Views
{
    public partial class DishManagementView : UserControl
    {
        public DishManagementView()
        {
            InitializeComponent();
            Loaded += DishManagementView_Loaded;
        }

        private void DishManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DishManagementViewModel viewModel)
            {
                // Handle file browse command - now this will work because BrowseImageCommand has a public setter
                viewModel.BrowseImageCommand = new RelayCommand(() =>
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif",
                        Title = "Select an image"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        viewModel.SelectedImagePath = openFileDialog.FileName;
                    }
                });
            }
        }
    }
}