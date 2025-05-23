using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantManager.Models
{
    public enum UserRoleType
    {
        Client = 1,
        Employee = 2
    }

    public class UserRole : INotifyPropertyChanged
    {
        private int _roleId;
        private string _roleName;
        private ObservableCollection<User> _users;

        public int RoleId
        {
            get => _roleId;
            set
            {
                if (_roleId != value)
                {
                    _roleId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RoleName
        {
            get => _roleName;
            set
            {
                if (_roleName != value)
                {
                    _roleName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Name)); // Notify Name property changed too
                }
            }
        }

        // Add Name property for compatibility with mapping/service code
        public string Name
        {
            get => RoleName;
            set
            {
                if (RoleName != value)
                {
                    RoleName = value; // This will also raise PropertyChanged for Name
                }
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserRole()
        {
            Users = new ObservableCollection<User>();
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class User : INotifyPropertyChanged
    {
        private int _userId;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _deliveryAddress;
        private string _passwordHash;
        private int _roleId;
        private UserRole _role;
        private ObservableCollection<Order> _orders;

        public int UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set
            {
                if (_deliveryAddress != value)
                {
                    _deliveryAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PasswordHash
        {
            get => _passwordHash;
            set
            {
                if (_passwordHash != value)
                {
                    _passwordHash = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RoleId
        {
            get => _roleId;
            set
            {
                if (_roleId != value)
                {
                    _roleId = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserRole Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    RoleId = value?.RoleId ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                if (_orders != value)
                {
                    _orders = value;
                    OnPropertyChanged();
                }
            }
        }

        public User()
        {
            Orders = new ObservableCollection<Order>();
        }

        public bool IsClient => RoleId == (int)UserRoleType.Client;
        public bool IsEmployee => RoleId == (int)UserRoleType.Employee;

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}