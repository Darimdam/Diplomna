using Diplomna.Resources.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Xml.Linq;

namespace Diplomna
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainMenuView : Window, IView
    {
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        public MainMenuView()
        {
            InitializeComponent();

            Timer.Tick += new EventHandler(Timer_Click);

            Timer.Interval = new TimeSpan(0, 0, 1);

            Timer.Start();
        }

        private void Timer_Click(object sender, EventArgs e)

        {

            DateTime d;

            d = DateTime.Now;

            Clock.Text = d.Hour + " : " + d.Minute + " ч.";

        }
    }
}
