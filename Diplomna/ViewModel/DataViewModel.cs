using Diplomna.Database.Models;
using Diplomna.Helpers;
using Diplomna.View;
using Diplomna.Database.Interfaces;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using Unity;

namespace Diplomna.ViewModel
{
    public class DataViewModel : BaseViewModel
    {
        #region Declarations
        private PlotModel model;
        private DataChartView dcv;
        private DateTime filterDateBegin;
        private DateTime filterDateEnd;
        private TimeIntervals selectedInterval;
        private List<Sensor> sensors;
        private List<SensorValue> sensorsValues;
        private ObservableCollection<CombinedData> combinedDatas;

        ManagementEventWatcher insertionWatcher;
        ManagementEventWatcher removalWatcher;

        private IApplicationDatabaseService dbService;

        private double? averageTemp;

        SerialPort serialPort;

        private ICommand filterCommand;
        private ICommand chartCommand;
        #endregion

        #region Init
        public DataViewModel(IUnityContainer container)
        {
            this.container = container;
            dbService = container.Resolve<IApplicationDatabaseService>();

            AverageTemp = 0;

            FilterDateBegin = DateTime.Today;
            FilterDateEnd = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);

            SelectedInterval = TimeIntervals.TenMinutes;

            FilterValues();

            var comPortName = AutodetectArduinoPort();
            ConnectToArduinoPort(comPortName);

            SetUpCOMConnection();
        }
        #endregion

        #region Commands
        public ICommand FilterCommand
        {
            get
            {
                if (filterCommand == null)
                    filterCommand = new BaseCommand(FilterValues);

                return filterCommand;
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
        #endregion

        #region Properties
        public DateTime FilterDateBegin
        {
            get
            {
                return filterDateBegin;
            }
            set
            {
                filterDateBegin = value;
                OnPropertyChanged(nameof(FilterDateBegin));
            }
        }

        public DateTime FilterDateEnd
        {
            get
            {
                return filterDateEnd;
            }
            set
            {
                filterDateEnd = value;
                OnPropertyChanged(nameof(FilterDateEnd));
            }
        }

        public TimeIntervals SelectedInterval
        {
            get
            {
                return selectedInterval;
            }
            set
            {
                if (selectedInterval == value) return;

                selectedInterval = value;
                OnPropertyChanged(nameof(SelectedInterval));
            }
        }

        public double? AverageTemp
        {
            get
            {
                return averageTemp;
            }

            set
            {
                averageTemp = value;
                OnPropertyChanged(nameof(AverageTemp));
            }
        }

        public List<Sensor> Sensors
        {
            get
            {
                if (sensors == null)
                    sensors = new List<Sensor>(dbService.GetSensors());

                return sensors;
            }
        }

        public List<SensorValue> SensorsValues
        {
            get
            {
                if (sensorsValues == null)
                    sensorsValues = dbService.GetFilteredValues(FilterDateBegin, FilterDateEnd);

                return sensorsValues;
            }
        }

        public ObservableCollection<CombinedData> CombinedDatas
        {
            get
            {
                if (combinedDatas == null)
                    combinedDatas = new ObservableCollection<CombinedData>(FilterDataByIntervals());

                return combinedDatas;
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
        #endregion

        #region Methods

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string incomingData = sp.ReadLine().TrimEnd('\r');

            if (incomingData != null)
            {
                incomingData = incomingData.Substring(0,16);
                string[] values = incomingData.Split('|');
                for (int i = 0; i <= 3; i++)
                {
                    try
                    {
                        var parsedTemp = double.Parse(values[i], CultureInfo.InvariantCulture);

                        dbService.AddSensorValue(i + 1, parsedTemp);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                FilterValues();
            }
        }

        private void ConnectToArduinoPort(string portName)
        {
            try
            {
                if (SerialPort.GetPortNames().ToList().Contains(portName))
                {
                    serialPort = new SerialPort(portName, 9600);
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    serialPort.Open();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                serialPort.Close();
                //ConnectToArduinoPort(portName);
                return;
            }
        }

        private string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return null;
        }

        private void SetUpCOMConnection()
        {
            // Subscribe to USB device insertion and removal events
            WqlEventQuery insertionQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            insertionWatcher = new ManagementEventWatcher(insertionQuery);
            insertionWatcher.EventArrived += USBInsertedEvent;
            insertionWatcher.Start();

            WqlEventQuery removalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            removalWatcher = new ManagementEventWatcher(removalQuery);
            removalWatcher.EventArrived += USBRemovedEvent;
        }

        private void USBInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            var comPortName = AutodetectArduinoPort();
            ConnectToArduinoPort(comPortName);

            insertionWatcher.Stop();
            removalWatcher.Start();
        }

        private void USBRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            WqlEventQuery insertionQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            insertionWatcher = new ManagementEventWatcher(insertionQuery);
            insertionWatcher.EventArrived += USBInsertedEvent;

            if (serialPort != null)
                serialPort.Close();

            insertionWatcher.Start();
            removalWatcher.Stop();
        }

        private void FilterValues(object param = null)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            SensorsValues.Clear();
            SensorsValues.AddRange(dbService.GetFilteredValues(FilterDateBegin, FilterDateEnd));

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                CombinedDatas.Clear();
                foreach (CombinedData cd in FilterDataByIntervals())
                {
                    CombinedDatas.Add(cd);
                }
            });

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        private List<CombinedData> FilterDataByIntervals()
        {
            List<CombinedData> combinedDatas = new List<CombinedData>();

            DateTime timeFrom = FilterDateBegin;
            TimeSpan timeSpan;

            foreach (DateTime time in GetIntervals())
            {
                combinedDatas.Add(new CombinedData
                {
                    Time = timeFrom,
                    FirstSensorValue = GetValueForInterval(1, timeFrom, time),
                    SecondSensorValue = GetValueForInterval(2, timeFrom, time),
                    ThirdSensorValue = GetValueForInterval(3, timeFrom, time),
                });

                timeSpan = time - timeFrom;
                timeFrom += timeSpan;
            }

            combinedDatas.Add(new CombinedData
            {
                Time = timeFrom,
                FirstSensorValue = null,
                SecondSensorValue = null,
                ThirdSensorValue = null,
            });

            AverageTemp = CalculateAverageTemp(combinedDatas);

            return combinedDatas;
        }

        private double? CalculateAverageTemp(List<CombinedData> combinedDatas)
        {
            double? averageTemp = 0;
            int valueCount = combinedDatas.Count()*3;

            foreach (CombinedData cd in combinedDatas)
            {
                averageTemp = cd.FirstSensorValue + cd.SecondSensorValue + cd.ThirdSensorValue;
            }

            return averageTemp / valueCount;
        }

        private double GetValueForInterval(int sensorId, DateTime timeFrom, DateTime timeTo)
        {
            int count = SensorsValues.Where(sv => sv.SensorId == sensorId && sv.Time >= timeFrom && sv.Time < timeTo).Count();
            return Math.Round(SensorsValues.Where(sv => sv.SensorId == sensorId && sv.Time >= timeFrom && sv.Time < timeTo).Sum(sv => sv.Value)
                    / (count == 0 ? 1 : count), 2);
        }

        private List<DateTime> GetIntervals()
        {
            List<DateTime> intervals = new List<DateTime>();
            DateTime beginDate = FilterDateBegin;
            int minutesToAdd = 10;

            switch (SelectedInterval)
            {
                case TimeIntervals.OneMinute:
                    minutesToAdd = 1;
                    break;
                case TimeIntervals.TenMinutes:
                    minutesToAdd = 10;
                    break;
                case TimeIntervals.OneHour:
                    minutesToAdd = 60;
                    break;
                case TimeIntervals.OneDay:
                    minutesToAdd = 1440;
                    break;
            }

            beginDate = beginDate.AddMinutes(minutesToAdd);

            while (beginDate <= FilterDateEnd)
            {
                intervals.Add(beginDate);
                beginDate = beginDate.AddMinutes(minutesToAdd);
            }

            if (beginDate > FilterDateEnd)
                intervals.Add(FilterDateEnd);

            return intervals;
        }

        private void SetDefaultAxes()
        {
            // X Axis
            var dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalType = DateTimeIntervalType.Days,
                Minimum = DateTimeAxis.ToDouble(FilterDateBegin),
                Maximum = DateTimeAxis.ToDouble(FilterDateEnd),
                Title = "Дата и час",
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

            Model.Axes.First().Minimum = DateTimeAxis.ToDouble(FilterDateBegin);
            Model.Axes.First().Maximum = DateTimeAxis.ToDouble(FilterDateEnd);

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

            var points1 = dbService.GetSensorValuesByDateAndSensorId(FilterDateBegin, FilterDateEnd, 1);
            var points2 = dbService.GetSensorValuesByDateAndSensorId(FilterDateBegin, FilterDateEnd, 2);
            var points3 = dbService.GetSensorValuesByDateAndSensorId(FilterDateBegin, FilterDateEnd, 3);

            var series1 = new LineSeries
            {
                TrackerKey = "Chart",
                Title = Sensors.First(s => s.Id == 1).Name + " (" + Sensors.First(s => s.Id == 1).SerialNumber + ")",
                DataFieldX = "Time",
                DataFieldY = "Value",
                MarkerSize = 3,
                MarkerStrokeThickness = 0.2,
                MarkerType = MarkerType.None,
                StrokeThickness = 2,
                Color = OxyColor.Parse("#106EBE"),
                MarkerFill = OxyColor.Parse("#106EBE"),
                MarkerStroke = OxyColor.Parse("#106EBE"),
                ItemsSource = points1
            };

            var series2 = new LineSeries
            {
                TrackerKey = "Chart",
                Title = Sensors.First(s => s.Id == 2).Name + " (" + Sensors.First(s => s.Id == 2).SerialNumber + ")",
                DataFieldX = "Time",
                DataFieldY = "Value",
                MarkerSize = 3,
                MarkerStrokeThickness = 0.2,
                MarkerType = MarkerType.None,
                StrokeThickness = 2,
                Color = OxyColor.Parse("#f7c92f"),
                MarkerFill = OxyColor.Parse("#f7c92f"),
                MarkerStroke = OxyColor.Parse("#f7c92f"),
                ItemsSource = points2
            };

            var series3 = new LineSeries
            {
                TrackerKey = "Chart",
                Title = Sensors.First(s => s.Id == 3).Name + " (" + Sensors.First(s => s.Id == 3).SerialNumber + ")",
                DataFieldX = "Time",
                DataFieldY = "Value",
                MarkerSize = 3,
                MarkerStrokeThickness = 0.2,
                MarkerType = MarkerType.None,
                StrokeThickness = 2,
                Color = OxyColor.Parse("#05b517"),
                MarkerFill = OxyColor.Parse("#05b517"),
                MarkerStroke = OxyColor.Parse("#05b517"),
                ItemsSource = points3
            };


            Model.Series.Clear();

            Model.Series.Add(series1);
            Model.Series.Add(series2);
            Model.Series.Add(series3);

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