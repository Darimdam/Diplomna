using AgroProgress.View;
using AgroProgress.ViewModel;
using Diplomna.Helpers;
using Diplomna.Resources.Interfaces;
using Diplomna.View;
using Diplomna.ViewModel;
using Unity;

namespace Diplomna.Services
{
    public static class FactoryService
    {
        public static IView GetContent(ViewModels vm, IUnityContainer container)
        {
            IView view;

            switch (vm)
            {
                case ViewModels.DataViewModel:
                    view = new DataView();
                    view.DataContext = new DataViewModel(container);
                    break;
                case ViewModels.ChartViewModel:
                    view = new ChartView();
                    view.DataContext = new ChartViewModel(container);
                    break;
                case ViewModels.LoginViewModel:
                    view = new LoginView();
                    view.DataContext = new LoginViewModel(container);
                    break;
                case ViewModels.RegisterViewModel:
                    view = new RegistrationView();
                    view.DataContext = new RegistrationViewModel(container);
                    break;
                case ViewModels.WeatherViewModel:
                    view = new WeatherView();
                    view.DataContext = new WeatherViewModel(container);
                    break;
                default:
                    view = null;
                    break;
            }

            return view;
        }
    }
}