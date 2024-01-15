using Diplomna.Helpers;
using Diplomna.Resources.Interfaces;
using Diplomna.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.XPath;

namespace Diplomna.ViewModel
{
    public class BaseMenuViewModel : BaseViewModel
    {
        #region Declarations
        protected IView currentView;
        private ICommand changeContentCommand;
        private ICommand launchProgramCommand;
        #endregion

        #region Commands
        public ICommand ChangeContentCommand
        {
            get
            {
                if (changeContentCommand == null)
                    changeContentCommand = new BaseCommand(this.ChangeView);

                return changeContentCommand;
            }
        }
        #endregion

        #region Properties
        public IView CurrentView
        {
            get
            {
                return currentView;
            }
            set
            {
                currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }
        #endregion

        #region Methods
        public void ChangeView(object param)
        {
            if (CurrentView != null)
                (CurrentView.DataContext as BaseViewModel).Dispose();
            CurrentView = FactoryService.GetContent((ViewModels)param, this.container);
        }

        #endregion
    }
}
