using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplomna.Database.Models
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public bool IsConnected { get; set; }

        public ICollection<SensorValue> SensorValues { get; set; }

    }
}