namespace Diplomna.Database.Migrations
{
    using Diplomna.Database.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Diplomna.Database.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Diplomna.Database.Models.ApplicationDbContext";
        }

        protected override void Seed(Diplomna.Database.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            context.Sensors.AddOrUpdate(
                    new Sensor { Id = 1, Name = "Сензор 1", SerialNumber = "DS18B20", IsConnected = true,},
                    new Sensor { Id = 2, Name = "Сензор 2", SerialNumber = "DS18B20", IsConnected = true,},
                    new Sensor { Id = 3, Name = "Сензор 3", SerialNumber = "DS18B20", IsConnected = true});
        }
    }
}
