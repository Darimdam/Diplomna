using Diplomna.Database.Models;
using Diplomna.Helpers;
using Diplomna.Services;
using Diplomna.Database.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Unity;

namespace Diplomna.ViewModel
{
    public class RegistrationViewModel : BaseViewModel, IDataErrorInfo
    {
        #region Declarations
        private ICommand registerCommand;
        private string password;
        private string username;
        private string confirmPassword;

        private IApplicationDatabaseService dbService;
        #endregion

        #region Init
        public RegistrationViewModel(IUnityContainer _container)
        {
            container = _container;
            dbService = container.Resolve<IApplicationDatabaseService>();
        }
        #endregion

        #region Commands
        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                    registerCommand = new BaseCommand(AddUser, CanExecuteAddUser);

                return registerCommand;
            }
        }
        #endregion

        #region Properties
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (username == value)
                    return;

                username = value;
                OnPropertyChanged("Username");
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password == PasswordCryptService.CryptPassword(value))
                    return;

                password = PasswordCryptService.CryptPassword(value);
                OnPropertyChanged("Password");
            }
        }

        public string ConfirmPassword
        {
            get
            {
                return confirmPassword;
            }
            set
            {
                if (confirmPassword == PasswordCryptService.CryptPassword(value))
                    return;

                confirmPassword = PasswordCryptService.CryptPassword(value);
                OnPropertyChanged("ConfirmPassword");
            }
        }
        #endregion

        #region Methods
        private void AddUser(object obj)
        {
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ConfirmPassword) && ConfirmPassword == Password)
            {
                dbService.AddUser(new User
                {
                    Name = Username,
                    Password = Password
                });

                App.CurrentViewModel.ChangeView(ViewModels.LoginViewModel);
            }
        }

        private bool CanExecuteAddUser()
        {
            return ErrorCollection.Count == 0;
        }
        #endregion

        #region Validation
        public string Error
        {
            get
            {
                return null;
            }
        }

        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();

        public string this[string param]
        {
            get
            {
                string result = null;

                switch (param)
                {
                    case "Username":
                        if (string.IsNullOrWhiteSpace(Username))
                            result = "Въведете име";
                        else if (dbService.GetUserByName(Username) != null)
                            result = "Съществува потребител със същото име";
                        break;
                    case "Password":
                        if (string.IsNullOrWhiteSpace(Password))
                            result = "Въведете парола";
                        break;
                    case "ConfirmPassword":
                        if (string.IsNullOrWhiteSpace(ConfirmPassword))
                            result = "Въведете паролата отново";
                        else if (Password != ConfirmPassword)
                            result = "Паролите не съвпадат";
                        break;
                }

                if (ErrorCollection.ContainsKey(param) && result != null)
                {
                    ErrorCollection[param] = result;
                }
                else if (ErrorCollection.ContainsKey(param) && result == null)
                {
                    ErrorCollection.Remove(param);
                }
                else if (result != null)
                {
                    ErrorCollection.Add(param, result);
                }

                OnPropertyChanged("ErrorCollection");

                return result;
            }
        }
        #endregion
    }
}