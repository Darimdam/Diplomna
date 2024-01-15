using Diplomna.Database.Interfaces;
using Diplomna.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diplomna.Database.Services
{
    public class ApplicationDatabaseService : IApplicationDatabaseService
    {
        private ApplicationDbContext dbContext;

        public ApplicationDatabaseService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return dbContext.Users;
        }

        public User GetUserByName(string name)
        {
            return dbContext.Users.FirstOrDefault(u => u.Name == name);
        }

        public IEnumerable<Sensor> GetSensors()
        {
            dbContext = new ApplicationDbContext();
            return dbContext.Sensors.OrderBy(s => s.Id).Take(4);
        }

        public List<SensorValue> GetSensorValuesFor3DaysBySensorId(int sensorId)
        {
            dbContext = new ApplicationDbContext();
            var fromDate = DateTime.Today.AddDays(-1);
            try
            {
                return dbContext.Values.Where(v => v.SensorId == sensorId && v.Time >= fromDate).ToList();
            }
            catch (Exception e)
            {
                return new List<SensorValue>();
            }
        }

        public SensorValue GetNextSensorValueByIdAndSensorId(int valueId, int sensorId)
        {
            dbContext = new ApplicationDbContext();
            try
            {
                return dbContext.Values.Where(v => v.Id > valueId && v.SensorId == sensorId).OrderByDescending(v => v.Id).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void AddSensorValue(int sensorId, double value)
        {
            dbContext = new ApplicationDbContext();
            dbContext.Values.Add(new SensorValue
            {
                Value = Math.Round(value, 0),
                Time = DateTime.Now,
                SensorId = sensorId
            });
            dbContext.SaveChanges();
        }

        public void AddUser(User user)
        {
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        public void SaveChanges()
        {
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception e) { }
        }

        public Image GetImage()
        {
            return dbContext.Images.FirstOrDefault();
        }

        public void AddImage(Image image)
        {
            dbContext.Images.RemoveRange(dbContext.Images);
            dbContext.Images.Add(image);
            dbContext.SaveChanges();
        }

        public List<SensorValue> GetFilteredValues(DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            return new List<SensorValue>(dbContext.Values.Where(v => v.Time >= dateTimeBegin && v.Time <= dateTimeEnd));
        }

        public List<SensorValue> GetValuesBySensor(int sensorId)
        {
            DateTime dayEarlier = DateTime.Now.AddDays(-1);
            return new List<SensorValue>(dbContext.Values.Where(v => v.SensorId == sensorId && v.Time <= DateTime.Now && v.Time >= dayEarlier));
        }

        public void DeleteOldData()
        {
            dbContext = new ApplicationDbContext();
            try
            {
                DateTime date = DateTime.Today.AddMonths(-1);
                dbContext.Values.RemoveRange(dbContext.Values.Where(v => v.Time < date));
                dbContext.SaveChanges();
            }
            catch (Exception e) { }
        }

        public void DisposeContext()
        {
            dbContext = new ApplicationDbContext();
            dbContext.Dispose();
        }

        public void UpdateSensors(List<Sensor> sensors)
        {
            foreach (var sensor in sensors)
            {
                var tmpSensor = dbContext.Sensors.Where(s => s.Id == sensor.Id).FirstOrDefault();
                if (tmpSensor != null)
                    tmpSensor.IsConnected = sensor.IsConnected;
            }
            dbContext.SaveChanges();
        }

        public List<SensorValue> GetSensorValuesByDateAndSensorId(DateTime fromDate, DateTime toDate, int sensorId)
        {
            return dbContext.Values.Where(v => v.SensorId == sensorId && v.Time >= fromDate && v.Time <= toDate).ToList();
        }
    }
}