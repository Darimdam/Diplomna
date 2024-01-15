using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Diplomna.Database.Models.WeatherInfo;

namespace Diplomna.Database.Models
{
    public class WeatherInfo
    {
        public class CurrentWeather
        {
            public double temperature { get; set; }
            public double windspeed { get; set; }
            public int winddirection { get; set; }
            public int weathercode { get; set; }
            public int is_day { get; set; }
            public string time { get; set; }
        }

        public class Daily
        {
            public List<string> time { get; set; }
            public List<double> temperature_2m_max { get; set; }
            public List<double> temperature_2m_min { get; set; }
            public List<string> sunrise { get; set; }
            public List<string> sunset { get; set; }
            public List<double> precipitation_sum { get; set; }
        }

        public class DailyUnits
        {
            public string time { get; set; }
            public string temperature_2m_max { get; set; }
            public string temperature_2m_min { get; set; }
            public string sunrise { get; set; }
            public string sunset { get; set; }
            public string precipitation_sum { get; set; }
        }

        public class Hourly
        {
            public List<string> time { get; set; }
            public List<double> temperature_2m { get; set; }
            public List<int> relativehumidity_2m { get; set; }
            public List<double> apparent_temperature { get; set; }
            public List<int> precipitation_probability { get; set; }
            public List<int> cloudcover { get; set; }
        }

        public class HourlyUnits
        {
            public string time { get; set; }
            public string temperature_2m { get; set; }
            public string relativehumidity_2m { get; set; }
            public string apparent_temperature { get; set; }
            public string precipitation_probability { get; set; }
            public string cloudcover { get; set; }
        }

        public class Root
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double generationtime_ms { get; set; }
            public int utc_offset_seconds { get; set; }
            public string timezone { get; set; }
            public string timezone_abbreviation { get; set; }
            public double elevation { get; set; }
            public CurrentWeather current_weather { get; set; }
            public HourlyUnits hourly_units { get; set; }
            public Hourly hourly { get; set; }
            public DailyUnits daily_units { get; set; }
            public Daily daily { get; set; }
        }
    }
}
