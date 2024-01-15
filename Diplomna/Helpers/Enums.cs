using System.ComponentModel;

namespace Diplomna.Helpers
{
    public enum ViewModels
    {
        Default = 0,
        ConfigurationsViewModel = 1,
        SettingsViewModel = 2,
        MapViewModel = 3,
        VideoViewModel = 4,
        DataViewModel = 5,
        ChartViewModel = 6,
        LoginViewModel = 8,
        RegisterViewModel = 9,
        AboutViewModel = 10,
        WarehouseViewModel = 11,
        WeatherViewModel = 12
    }

    public enum TimeIntervals
    {
        [Description("1 минута")]
        OneMinute = 1,
        [Description("10 минути")]
        TenMinutes = 2,
        [Description("1 час")]
        OneHour = 3,
        [Description("1 ден")]
        OneDay = 4
    }

    public enum MediaButtonTypes
    {
        Default = -1,
        Play = 0,
        Stop = 1,
        Pause = 2,
        Forward = 3,
        Backward = 4,
        Mute = 5
    }
}
