using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroProgress.Resources.WeatherApi
{
    public class Day
    {
        public string Name { get; set; } = "";
        public string Image { get; set; }
        public string Time { get; set; }
        public double Temperature_2m_max { get; set; }
        public double Temperature_2m_min { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public double Precipitation_sum { get; set; }
    }
}
