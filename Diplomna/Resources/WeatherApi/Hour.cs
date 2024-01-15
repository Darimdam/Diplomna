using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroProgress.Resources.WeatherApi
{
    public class Hour
    {
        public string Image { get; set; }
        public string Time { get; set; }
        public double Temperature_2m { get; set; }
        public int Relativehumidity_2m { get; set; }
        public double Apparent_temperature { get; set; }
        public int Precipitation_probability { get; set; }
        public int Cloudcover { get; set; }
    }
}
