using Diplomna.Database.Models;
using Diplomna.Helpers;
using Diplomna.Services;
using Diplomna.View;
using Diplomna.Database.Interfaces;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Unity;

namespace Diplomna.ViewModel
{
    public class ChartViewModel : BaseViewModel
    {
        #region Declarations
        private PlotModel model;
        private List<DateValue> points;
        private Sensor selectedSensor;
        private List<Sensor> sensors;
        private LineSeries series;

        private ICommand chromaCommand;
        private Random random;

        private int lastValueId;

        private IApplicationDatabaseService dbService;

        private CancellationTokenSource canceller;
        #endregion

        #region Init
        public ChartViewModel(IUnityContainer container, int sensorId = 1)
        {
            this.container = container;
            dbService = container.Resolve<IApplicationDatabaseService>();
            random = new Random();

            Model = new PlotModel();

            SetDefaultAxes();
            SelectedSensor = Sensors.Skip(sensorId - 1).First();
        }
        #endregion

        #region Properties
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

        public List<DateValue> Points
        {
            get
            {
                if (points == null)
                    points = new List<DateValue>();

                return points;
            }
            set
            {
                points = value;
            }
        }

        public Sensor SelectedSensor
        {
            get
            {
                return selectedSensor;
            }
            set
            {
                if (selectedSensor == value)
                    return;

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                selectedSensor = value;

                if (canceller != null)
                    canceller.Cancel();
                Thread.Sleep(500);
                if (selectedSensor.Name == "Всички")
                {
                    SetDefaultValuesForAllSensors();
                }
                else
                {
                    SetDefaultValues();
                    if (selectedSensor.IsConnected
                        && SensorsDataService.RunningSensors.ContainsKey(selectedSensor.Id)
                        && SensorsDataService.RunningSensors[selectedSensor.Id])
                        StartPointsTask();
                }

                OnPropertyChanged("SelectedSensor");

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        public List<Sensor> Sensors
        {
            get
            {
                if (sensors == null)
                {
                    sensors = new List<Sensor>(dbService.GetSensors());
                    sensors.Add(new Sensor() { Name = "Всички", SerialNumber = string.Empty });
                }

                return sensors;
            }
        }

        public LineSeries Series
        {
            get
            {
                if (series == null)
                    series = new LineSeries
                    {
                        TrackerKey = "Chart",
                        Title = SelectedSensor.Name + " (" + SelectedSensor.SerialNumber + ")",
                        DataFieldX = "Time",
                        DataFieldY = "Value",
                        MarkerSize = 3,
                        MarkerStrokeThickness = 0.2,
                        MarkerType = MarkerType.None,
                        StrokeThickness = 2,
                        Color = OxyColor.Parse("#106EBE"),
                        MarkerFill = OxyColor.Parse("#106EBE"),
                        MarkerStroke = OxyColor.Parse("#106EBE"),
                        ItemsSource = Points
                    };

                return series;
            }
            set
            {
                series = value;
            }
        }

        #endregion

        #region Methods
        private void SetDefaultAxes()
        {

            // X Axis
            var dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalType = DateTimeIntervalType.Days,
                Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddHours(-2)),
                Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddHours(1)),
                Title = "Дата и час",
                StringFormat = "dd/MM/yyyy\n     HH:mm"
            };
            model.Axes.Add(dateTimeAxis);

            // Y Axis
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -15,
                Maximum = 50,
                IsZoomEnabled = true,
                Title = "°C"
            };
            model.Axes.Add(valueAxis);

            // Y=0 Axis
            var annotation = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Color = OxyColor.Parse("#8C8B8B"),
                LineStyle = LineStyle.LongDashDotDot,
                Y = 0
            };
            model.Annotations.Add(annotation);

        }

        private void SetDefaultValues()
        {
            Model.Series.Clear();
            Model.Series.Add(Series);

            Points.RemoveAll(p => true);

            foreach (var value in dbService.GetSensorValuesFor3DaysBySensorId(SelectedSensor.Id))
            {
                lastValueId = value.Id;
                Points.Add(new DateValue { Time = value.Time, Value = value.Value });
            }

            if (Series != null)
                Series.Title = SelectedSensor.Name + " (" + SelectedSensor.SerialNumber + ")";

            if (Model != null)
                Model.InvalidatePlot(true);
        }

        private void SetDefaultValuesForAllSensors()
        {
            var points1 = dbService.GetSensorValuesFor3DaysBySensorId(1);
            var points2 = dbService.GetSensorValuesFor3DaysBySensorId(2);
            var points3 = dbService.GetSensorValuesFor3DaysBySensorId(3);

            int lastValueId1 = points1.Last().Id;
            int lastValueId2 = points2.Last().Id;
            int lastValueId3 = points3.Last().Id;

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

            canceller = new CancellationTokenSource();

            if (Sensors.Where(s => s.Id == 1).First().IsConnected
                && SensorsDataService.RunningSensors.ContainsKey(1)
                && SensorsDataService.RunningSensors[1])
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (canceller.Token.IsCancellationRequested)
                            break;

                        var newValue = dbService.GetNextSensorValueByIdAndSensorId(lastValueId1, 1);
                        if (newValue != null)
                        {
                            if (points1.Count > 0 && points1.First().Time < DateTime.Today.AddDays(-3))
                                points1.Remove(points1.First());

                            lastValueId1 = newValue.Id;
                            points1.Add(newValue);
                            Model.InvalidatePlot(true);

                            OnPropertyChanged("Model");
                            Thread.Sleep(2000);
                        }
                    }
                });

            if (Sensors.Where(s => s.Id == 2).First().IsConnected
                && SensorsDataService.RunningSensors.ContainsKey(2)
                && SensorsDataService.RunningSensors[2])
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (canceller.Token.IsCancellationRequested)
                            break;

                        var newValue = dbService.GetNextSensorValueByIdAndSensorId(lastValueId2, 2);
                        if (newValue != null)
                        {
                            if (points2.Count > 0 && points2.First().Time < DateTime.Today.AddDays(-3))
                                points2.Remove(points2.First());

                            lastValueId2 = newValue.Id;
                            points2.Add(newValue);
                            Model.InvalidatePlot(true);
                            Thread.Sleep(2000);
                        }
                    }
                });

            if (Sensors.Where(s => s.Id == 3).First().IsConnected
                && SensorsDataService.RunningSensors.ContainsKey(3)
                && SensorsDataService.RunningSensors[3])
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (canceller.Token.IsCancellationRequested)
                            break;

                        var newValue = dbService.GetNextSensorValueByIdAndSensorId(lastValueId3, 3);
                        if (newValue != null)
                        {
                            if (points3.Count > 0 && points3.First().Time < DateTime.Today.AddDays(-3))
                                points3.Remove(points3.First());

                            lastValueId3 = newValue.Id;
                            points3.Add(newValue);

                            Model.InvalidatePlot(true);
                            Thread.Sleep(2000);
                        }
                    }
                });

            if (Model != null)
                Model.InvalidatePlot(true);
        }

        private void StartPointsTask()
        {
            canceller = new CancellationTokenSource();

            Task.Run(() =>
            {
                while (true)
                {
                    if (canceller.Token.IsCancellationRequested)
                        break;

                    var newValue = dbService.GetNextSensorValueByIdAndSensorId(lastValueId, SelectedSensor.Id);
                    if (newValue != null)
                    {
                        if (Points.Count > 0 && Points.First().Time < DateTime.Today.AddDays(-3))
                            Points.Remove(Points.First());

                        lastValueId = newValue.Id;
                        Points.Add(new DateValue { Time = newValue.Time, Value = newValue.Value });
                        Model.InvalidatePlot(true);
                        Thread.Sleep(2000);
                    }
                }
            });
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {
            base.Dispose();
            if (canceller != null && !canceller.Token.IsCancellationRequested)
                canceller.Cancel();
            dbService.DisposeContext();
        }
        #endregion
    }
}
