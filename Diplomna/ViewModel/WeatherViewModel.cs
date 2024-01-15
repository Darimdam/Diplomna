using AgroProgress.Resources.WeatherApi;
using CommunityToolkit.Mvvm.Messaging;
using Diplomna.Database.Models;
using Diplomna.Helpers;
using Diplomna.View;
using Diplomna.ViewModel;
using Diplomna.Database.Models;
using Newtonsoft.Json;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Unity;
using static Diplomna.Database.Models.WeatherInfo;
using Diplomna.Database.Interfaces;
using OxyPlot.Series;
using System.Windows;

namespace AgroProgress.ViewModel
{
    public class WeatherViewModel : BaseViewModel
    {
        #region Definitions

        private string ApiKey = "4214cf23a1d243bf0bd20751e4f1e69b";

        private string currentWeatherImageSource;
        private string city;
        private string weatherType;
        private string temp;
        private string tempFeelsLike;
        private string tempMin;
        private string tempMax;
        private string pressure;
        private string humidity;
        private string windSpeed;
        private string clouds;
        private string picIcon;
        private Root info;

        private IApplicationDatabaseService dbService;
        private DataChartView dcv;
        private PlotModel model;

        private DateTime filterDateBegin;
        private DateTime filterDateEnd;

        private ObservableCollection<Hour> hourly;
        private ObservableCollection<Day> days;

        private ICommand chartCommand;
        #endregion

        #region Constructor
        public WeatherViewModel(IUnityContainer _container)
        {
            container = _container;
            dbService = container.Resolve<IApplicationDatabaseService>();

            ChartStartDate = DateTime.Today;
            ChartEndDate = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);

            Timer = new System.Windows.Threading.DispatcherTimer();
            StartTimer();

        }
        #endregion

        #region Properties

        System.Windows.Threading.DispatcherTimer Timer { get; set; }

        public string CurrentWeatherImageSource
        {
            get
            {
                return currentWeatherImageSource;
            }
            set
            {
                currentWeatherImageSource = value;
                OnPropertyChanged(nameof(CurrentWeatherImageSource));
            }
        }

        public PlotModel Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
            }
        }

        public DateTime ChartStartDate
        {
            get
            {
                return filterDateBegin;
            }
            set
            {
                filterDateBegin = value;
                OnPropertyChanged(nameof(ChartStartDate));
            }
        }

        public DateTime ChartEndDate
        {
            get
            {
                return filterDateEnd;
            }
            set
            {
                filterDateEnd = value;
                OnPropertyChanged(nameof(ChartEndDate));
            }
        }

        public ICommand ChartCommand
        {
            get
            {
                if (chartCommand == null)
                    chartCommand = new BaseCommand(ShowChart);

                return chartCommand;
            }
        }

        public string City
        {
            get
            {
                return city;
            }
            set
            {
                city = value;
                OnPropertyChanged(nameof(City));
            }
        }
        public string WeatherType
        {
            get
            {
                return weatherType;
            }
            set
            {
                weatherType = value;
                OnPropertyChanged(nameof(WeatherType));
            }
        }
        public string Temp
        {
            get
            {
                return temp;
            }
            set
            {
                temp = value;
                OnPropertyChanged(nameof(Temp));
            }
        }
        public string TempFeelsLike
        {
            get
            {
                return tempFeelsLike;
            }
            set
            {
                tempFeelsLike = value;
                OnPropertyChanged(nameof(TempFeelsLike));
            }
        }
        public string TempMin
        {
            get
            {
                return tempMin;
            }
            set
            {
                tempMin = value;
                OnPropertyChanged(nameof(TempMin));
            }
        }
        public string TempMax
        {
            get
            {
                return tempMax;
            }
            set
            {
                tempMax = value;
                OnPropertyChanged(nameof(TempMax));
            }
        }
        public string Precipitation
        {
            get
            {
                return pressure;
            }
            set
            {
                pressure = value;
                OnPropertyChanged(nameof(Precipitation));
            }
        }
        public string Humidity
        {
            get
            {
                return humidity;
            }
            set
            {
                humidity = value;
                OnPropertyChanged(nameof(Humidity));
            }
        }
        public string WindSpeed
        {
            get
            {
                return windSpeed;
            }
            set
            {
                windSpeed = value;
                OnPropertyChanged(nameof(WindSpeed));
            }
        }
        public string Clouds
        {
            get
            {
                return clouds;
            }
            set
            {
                clouds = value;
                OnPropertyChanged(nameof(Clouds));
            }
        }

        public string PicIcon
        {
            get
            {
                return picIcon;
            }
            set
            {
                picIcon = value;
                OnPropertyChanged(nameof(PicIcon));
            }
        }

        public ObservableCollection<Hour> Hourly
        {
            get
            {
                return hourly;
            }
            set
            {
                hourly = value;
                OnPropertyChanged(nameof(Hourly));
            }
        }

        public ObservableCollection<Day> Days
        {
            get
            {
                return days;
            }
            set
            {
                days = value;
                OnPropertyChanged(nameof(Days));
            }
        }

        public Root Info
        {
            get 
            {
                return info;
            }
            set
            {
                info = value;
                OnPropertyChanged(nameof(Info));
            }
        }

        #endregion

        #region Methods

        private async Task GetWeather()
        {
            using (WebClient web = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var startDate = DateTime.Now.ToString("yyyy-MM-dd");
                var endDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

                string url = String.Format("https://api.open-meteo.com/v1/forecast?latitude=42.6975&longitude=23.3241&hourly=temperature_2m,relativehumidity_2m,apparent_temperature,precipitation_probability,cloudcover&daily=temperature_2m_max,temperature_2m_min,sunrise,sunset,precipitation_sum&current_weather=true&timezone=auto&{0}&{1}&models=best_match", startDate, endDate);
                var json = web.DownloadString(url);
                Info = JsonConvert.DeserializeObject<Root>(json);

                CurrentWeatherImageSource = GetImageAddres(Info.current_weather.is_day, Info.hourly.precipitation_probability.Max());
                Precipitation = Info.hourly.precipitation_probability.Max().ToString();
                Temp = Info.current_weather.temperature.ToString();
                TempFeelsLike = Info.hourly.apparent_temperature.FirstOrDefault().ToString();
                WindSpeed = Info.current_weather.windspeed.ToString();
                Humidity = Info.hourly.relativehumidity_2m.FirstOrDefault().ToString();
                TempMin = Info.daily.temperature_2m_min.FirstOrDefault().ToString();
                TempMax = Info.daily.temperature_2m_max.FirstOrDefault().ToString();
                Clouds = Info.hourly.cloudcover.FirstOrDefault().ToString();

                Days = new ObservableCollection<Day>();
                Hourly = new ObservableCollection<Hour>();

                try
                {
                    for (int i = 0; i < Info.daily.temperature_2m_max.Count; i++)
                    {
                        Days.Add(new Day
                        {
                            Name = DateTime.Now.AddDays(i).DayOfWeek.ToString("g"),
                            Time = Info.daily.time[i],
                            Temperature_2m_max = Info.daily.temperature_2m_max[i],
                            Temperature_2m_min = Info.daily.temperature_2m_min[i],
                            Sunrise = Info.daily.sunrise[i].Substring(11, 5),
                            Sunset = Info.daily.sunset[i].Substring(11, 5),
                            Precipitation_sum = Info.daily.precipitation_sum[i]
                        }); ;
                    }

                    var currentHour = DateTime.Now.ToString("yyyy-MM-ddTHH:00");

                    var index = Info.hourly.time.IndexOf(currentHour);

                    for (int i = index; i < index + 4; i++)
                    {
                        var isDay = DateTime.Parse(currentHour).Hour > 19 ? 0 : 1;

                        Hourly.Add(new Hour
                        {
                            Image = GetImageAddres(isDay, Info.hourly.cloudcover[i]),
                            Time = Info.hourly.time[i].Substring(11, 5),
                            Temperature_2m = Info.hourly.temperature_2m[i],
                            Relativehumidity_2m = Info.hourly.relativehumidity_2m[i],
                            Apparent_temperature = Info.hourly.apparent_temperature[i],
                            Precipitation_probability = Info.hourly.precipitation_probability[i],
                            Cloudcover = Info.hourly.cloudcover[i]
                        });
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private string GetImageAddres(int isDay, double precipitation)
        {
            var image = "";

            switch(precipitation)
            {
                case var expression when precipitation < 20:
                    image = "01";
                    break;
                case var expression when (precipitation > 20 && precipitation < 40):
                    image = "02";
                    break;
                case var expression when (precipitation > 40 && precipitation < 80):
                    image = "03";
                    break;
                case var expression when precipitation > 80:
                    image = "04";
                    break;
            }

            if (isDay == 0)
                image = image + "n";
            else
                image = image + "d";

            return "/Resources/Images/" + image + ".png";
        }

        private void StartTimer()
        {
            GetWeather();

            Timer.Tick += new EventHandler(Timer_Click);

            Timer.Interval = new TimeSpan(0, 15, 0);

            Timer.Start();
        }

        private async void Timer_Click(object sender, EventArgs e)
        {
            await GetWeather();
        }

        private void SetDefaultAxes()
        {
            // X Axis
            var dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalType = DateTimeIntervalType.Hours,
                Minimum = DateTimeAxis.ToDouble(ChartStartDate),
                Maximum = DateTimeAxis.ToDouble(ChartEndDate),
                Title = "Час",
                StringFormat = "dd/MM/yyyy\n     HH:mm"
            };
            model.Axes.Add(dateTimeAxis);

            // Y Axis
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -15,
                Maximum = 30,
                IsZoomEnabled = false,
                Title = "°C"
            };
            model.Axes.Add(valueAxis);
        }

        private void ShowChart(object obj)
        {
            Model = new PlotModel();
            SetDefaultAxes();

            Model.Axes.First().Minimum = DateTimeAxis.ToDouble(ChartStartDate);
            Model.Axes.First().Maximum = DateTimeAxis.ToDouble(ChartEndDate);

            Model.Annotations.Clear();

            // Y=0 Axis
            var annotation = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Color = OxyColor.Parse("#8C8B8B"),
                LineStyle = LineStyle.LongDashDotDot,
                Y = 0
            };
            model.Annotations.Add(annotation);

            //Mapping values from the API 
            var temp = new List<ChartData>();
            var apparentTemp = new List<ChartData>();

            for(int i = 0; i < Info.hourly.temperature_2m.Count; i++)
            {
                temp.Add(new ChartData
                { 
                    Time = DateTime.Parse(Info.hourly.time[i]),
                    Temp = Info.hourly.temperature_2m[i]
                });

                apparentTemp.Add(new ChartData
                {
                    Time = DateTime.Parse(Info.hourly.time[i]),
                    Temp = Info.hourly.apparent_temperature[i]
                });
            }

            var series1 = new LineSeries
            {
                TrackerKey = "Chart",
                Title = "Температура",
                DataFieldX = "Time",
                DataFieldY = "Temp",
                MarkerSize = 3,
                MarkerStrokeThickness = 0.2,
                MarkerType = MarkerType.None,
                StrokeThickness = 2,
                Color = OxyColor.Parse("#106EBE"),
                MarkerFill = OxyColor.Parse("#106EBE"),
                MarkerStroke = OxyColor.Parse("#106EBE"),
                ItemsSource = temp
            };

            var series2 = new LineSeries
            {
                TrackerKey = "Chart",
                Title = "Усеща се като",
                DataFieldX = "Time",
                DataFieldY = "Temp",
                MarkerSize = 3,
                MarkerStrokeThickness = 0.2,
                MarkerType = MarkerType.None,
                StrokeThickness = 2,
                Color = OxyColor.Parse("#f7c92f"),
                MarkerFill = OxyColor.Parse("#f7c92f"),
                MarkerStroke = OxyColor.Parse("#f7c92f"),
                ItemsSource = apparentTemp
            };

            Model.Series.Clear();

            Model.Series.Add(series1);
            Model.Series.Add(series2);

            Model.InvalidatePlot(true);

            try
            {
                dcv = new DataChartView();
                dcv.DataContext = this;

                dcv.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion
    }
}

public class ChartData
{
    public double Temp { get; set; }
    public DateTime Time { get; set; }
}
