using AgroProgress.ViewModel;
using CommunityToolkit.Mvvm.Messaging;
using Diplomna.Database.Models;
using Diplomna.Resources.Interfaces;
using Diplomna.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Diplomna.Database.Models.WeatherInfo;

namespace AgroProgress.View
{
    /// <summary>
    /// Interaction logic for WeatherView.xaml
    /// </summary>
    public partial class WeatherView : UserControl, IView
    {
        public WeatherView()
        {
            InitializeComponent();
        }
    }
}
