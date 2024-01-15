using Diplomna.Services;
using Diplomna.View;
using Diplomna.ViewModel;
using Diplomna.Database.Interfaces;
using Diplomna.Database.Services;
using System.Windows;
using Unity;

namespace Diplomna
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BaseMenuViewModel CurrentViewModel { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IApplicationDatabaseService, ApplicationDatabaseService>();

            CurrentViewModel = container.Resolve<UserMenuViewModel>();
            UserMenuView window = new UserMenuView { DataContext = CurrentViewModel };

            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SensorsDataService.StopRunningSensors();

            base.OnExit(e);
        }
    }
}
