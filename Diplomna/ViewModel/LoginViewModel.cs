using CommunityToolkit.Mvvm.Messaging;
using Diplomna.Helpers;
using Diplomna.Services;
using Diplomna.Database.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace Diplomna.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        #region Declarations
        private ICommand loginCommand;
        private string password;
        private string selectedUsername;
        private string errorMessage;

        private IApplicationDatabaseService dbService;
        #endregion

        #region Init
        public LoginViewModel(IUnityContainer _container)
        {
            container = _container;
            errorMessage = string.Empty;
            dbService = container.Resolve<IApplicationDatabaseService>();
        }
        #endregion

        #region Commands
        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                    loginCommand = new BaseCommand(LoginUser, CanExecuteLoginUser);

                return loginCommand;
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<string> Usernames 
        { 
            get
            {
                return new ObservableCollection<string>(dbService.GetAllUsers().Select(usr => usr.Name));
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
                if (password == value)
                    return;

                password = value;
                OnPropertyChanged("Password");
            }
        }
        public string SelectedUsername
        {
            get
            {
                if (selectedUsername == null)
                    selectedUsername = Usernames.LastOrDefault();

                return selectedUsername;
            }
            set
            {
                if (selectedUsername == value)
                    return;

                selectedUsername = value;
                OnPropertyChanged("SelectedUsername");
            }
        }

        public string ErrorMessage {
            get
            {
                return errorMessage;
            }
            set
            {
                if (errorMessage == value)
                    return;

                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }
        #endregion

        #region Methods
        public void LoginUser(object obj)
        {
            ErrorMessage = string.Empty;

            if (!string.IsNullOrEmpty(SelectedUsername) && !string.IsNullOrEmpty(Password)) 
            {
                var user = dbService.GetAllUsers()
                    .FirstOrDefault(u => u.Name == SelectedUsername && u.Password == PasswordCryptService.CryptPassword(Password));

                if (user != null)
                {
                    LoggedUser.User = user;

                    var vm = container.Resolve<MainMenuViewModel>();
                    MainMenuView window = new MainMenuView { DataContext = vm };
                    window.Show();

                    var mainWindow = App.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext.GetType() == typeof(UserMenuViewModel));
                    if (mainWindow != null)
                        mainWindow.Close();

                    SensorsDataService.StopRunningSensors();
                    SensorsDataService.RunConnectedSensors();

                    WeakReferenceMessenger.Default.Send(user);
                }
                else
                {
                    ErrorMessage = "Грешна парола!";
                }
            }
            else
            {
                ErrorMessage = "Грешна парола!";
            }
        }

        private bool CanExecuteLoginUser()
        {
            return SelectedUsername != null;
        }
        #endregion
    }
}