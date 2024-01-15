using CommunityToolkit.Mvvm.Messaging;
using Diplomna.Database.Models;
using Diplomna.Helpers;
using Diplomna.Services;
using Diplomna.View;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace Diplomna.ViewModel
{
    public class MainMenuViewModel : BaseMenuViewModel
    {
        #region Declarations
        private ICommand logOutCommand;

        private string username;
        #endregion

        #region Init
        public MainMenuViewModel(IUnityContainer container)
        {
            this.container = container;
            WeakReferenceMessenger.Default.Register<User>(this, (r, m) => Username = m.Name);
        }
        #endregion

        #region Commands
        public ICommand LogOutCommand
        {
            get
            {
                if (logOutCommand == null)
                    logOutCommand = new BaseCommand(LogOut, CanExecuteLogOut);

                return logOutCommand;
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
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        #endregion

        #region Methods
        private void LogOut(object param)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            SensorsDataService.StopRunningSensors();

            var vm = container.Resolve<UserMenuViewModel>();
            UserMenuView window = new UserMenuView { DataContext = vm };
            window.Show();

            var mainWindow = App.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext.GetType() == typeof(MainMenuViewModel));
            if (mainWindow != null)
                mainWindow.Close();

            LoggedUser.User = null;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        private bool CanExecuteLogOut()
        {
            return LoggedUser.User != null;
        }
        #endregion
    }
}