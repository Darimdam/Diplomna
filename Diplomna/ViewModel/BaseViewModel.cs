using System;
using System.ComponentModel;
using Unity;

namespace Diplomna.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Declarations
        protected IUnityContainer container;
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Disposable
        //TODO - да се имплементира за всички viewmodel-и
        public virtual void Dispose()
        {
        }
        #endregion
    }
}
