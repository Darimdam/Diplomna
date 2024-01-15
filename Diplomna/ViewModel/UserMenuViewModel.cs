using Diplomna.Helpers;
using Diplomna.Services;
using Unity;

namespace Diplomna.ViewModel
{
    public class UserMenuViewModel : BaseMenuViewModel
    {
        #region Declarations
        #endregion

        #region Init
        public UserMenuViewModel(IUnityContainer _container)
        {
            this.container = _container;
            this.currentView = FactoryService.GetContent(ViewModels.LoginViewModel, this.container);
        }
        #endregion

        #region Commands
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
    }
}