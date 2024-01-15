using Diplomna.Database.Models;
using System;
using System.Collections.Generic;

namespace Diplomna.Database.Interfaces
{
    public interface IApplicationDatabaseService
    {
        IEnumerable<User> GetAllUsers();

        User GetUserByName(string name);

        void AddUser(User user);

        IEnumerable<Sensor> GetSensors();

        List<SensorValue> GetSensorValuesFor3DaysBySensorId(int sensorId);

        SensorValue GetNextSensorValueByIdAndSensorId(int valueId, int sensorId);

        void AddSensorValue(int sensorId, double value);

        void SaveChanges();

        Image GetImage();

        void AddImage(Image image);

        List<SensorValue> GetFilteredValues(DateTime dateTimeBegin, DateTime dateTimeEnd);

        List<SensorValue> GetValuesBySensor(int sensorId);

        void DeleteOldData();

        void DisposeContext();

        void UpdateSensors(List<Sensor> sensors);

        List<SensorValue> GetSensorValuesByDateAndSensorId(DateTime fromDate, DateTime toDate, int sensorId);
    }
}
