using Diplomna.Database.Models;
using Diplomna.Database.Interfaces;
using Diplomna.Database.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Diplomna.Services
{
    public static class SensorsDataService
    {
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        //YAPI Hub
        private static string errmsg = "";

        public static Dictionary<int, bool> RunningSensors { get; } = new Dictionary<int, bool>();

        public static void StopRunningSensors()
        {
            tokenSource.Cancel();
            if (RunningSensors.Count > 0 && RunningSensors.ContainsValue(true))
                YAPI.Sleep(2000, ref errmsg);
            YAPI.FreeAPI();
        }

        public static void RunConnectedSensors()
        {
            var dbService = new ApplicationDatabaseService(null);
            tokenSource = new CancellationTokenSource();

            //dbService.DeleteOldData();

            YAPI.RegisterHub("usb", ref errmsg);
            var sensors = dbService.GetSensors();

            foreach (var sensor in sensors)
                if (sensor.IsConnected)
                {
                    try
                    {
                        var tmp = YLightSensor.FindLightSensor(sensor.SerialNumber + ".lightSensor").FriendlyName;
                    }
                    catch (Exception)
                    {
                        if (RunningSensors.ContainsKey(sensor.Id))
                            RunningSensors[sensor.Id] = false;

                        continue;
                    }
                    Task.Run(() =>
                    {
                        CollectDataTask(sensor, sensor.SerialNumber);
                    }, tokenSource.Token);
                }
        }

        private static void CollectDataTask(Sensor sensor, string serialNumber)
        {
            var dbService = new ApplicationDatabaseService(null);

            YLightSensor lightSensor = YLightSensor.FindLightSensor(serialNumber + ".lightSensor");

            if (sensor.Id != 0)
                if (RunningSensors.ContainsKey(sensor.Id))
                    RunningSensors[sensor.Id] = true;
                else
                    RunningSensors.Add(sensor.Id, true);

            while (true)
            {
                if (tokenSource.Token.IsCancellationRequested)
                {
                    if (RunningSensors.ContainsKey(sensor.Id))
                        RunningSensors[sensor.Id] = false;
                    break;
                }

                try
                {
                    dbService.AddSensorValue(sensor.Id, lightSensor.get_currentRawValue());
                    YAPI.Sleep(2000, ref errmsg);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Device not found")
                        && RunningSensors.ContainsKey(sensor.Id))
                    {
                        RunningSensors[sensor.Id] = false;
                        break;
                    }
                }

                //dbService.DeleteOldData();
            }
        }
    }
}