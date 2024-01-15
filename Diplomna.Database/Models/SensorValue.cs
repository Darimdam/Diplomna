using System;
using System.ComponentModel.DataAnnotations;


namespace Diplomna.Database.Models
{
    public class SensorValue
    {
        [Key]
        public int Id { get; set; }
        public double Value { get; set; }
        public DateTime Time { get; set; }
        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }

    }
}
