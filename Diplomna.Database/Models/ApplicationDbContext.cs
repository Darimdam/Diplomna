using Diplomna.Database.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Diplomna.Database.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=connectionString")
        {
            if (Database.CreateIfNotExists())
            {
                if (Sensors.Count() == 0)
                {
                    Sensors.AddOrUpdate(
                    new Sensor { Id = 1, Name = "Сензор 1", SerialNumber = "DS18B20", IsConnected = true},
                    new Sensor { Id = 2, Name = "Сензор 2", SerialNumber = "DS18B20", IsConnected = true },
                    new Sensor { Id = 3, Name = "Сензор 3", SerialNumber = "DS18B20", IsConnected = true });
                }

                SaveChanges();
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorValue> Values { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}