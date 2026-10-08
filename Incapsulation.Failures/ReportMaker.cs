using System;
using System.Collections.Generic;
using System.Linq;

namespace Incapsulation.Failures
{
    /// <summary>
    /// Типы сбоев устройств
    /// </summary>
    public enum FailureType
    {
        UnexpectedShutdown = 0,
        ShortNonResponding = 1,
        HardwareFailure = 2,
        ConnectionProblems = 3
    }

    /// <summary>
    /// Сущность "Сбой"
    /// </summary>
    public class Failure
    {
        public FailureType Type { get; }
        public DateTime Date { get; }
        public int DeviceId { get; }

        public Failure(FailureType type, DateTime date, int deviceId)
        {
            Type = type;
            Date = date;
            DeviceId = deviceId;
        }

        public bool IsSerious => (int)Type % 2 == 0;
    }

    /// <summary>
    /// Сущность "Устройство"
    /// </summary>
    public class Device
    {
        public int Id { get; }
        public string Name { get; }

        public Device(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class ReportMaker
    {
        /// <summary>
        /// Новый метод с инкапсуляцией данных
        /// </summary>
        public static List<string> FindDevicesFailedBeforeDate(
            DateTime date,
            IEnumerable<Failure> failures,
            IEnumerable<Device> devices)
        {
            var problematicDeviceIds = failures
                .Where(f => f.IsSerious && f.Date < date)
                .Select(f => f.DeviceId)
                .Distinct()
                .ToHashSet();

            return devices
                .Where(d => problematicDeviceIds.Contains(d.Id))
                .Select(d => d.Name)
                .ToList();
        }

        /// <summary>
        /// Старый метод для обратной совместимости
        /// </summary>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes,
            int[] deviceId,
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            var date = new DateTime(year, month, day);

            var failures = new List<Failure>();
            for (int i = 0; i < failureTypes.Length; i++)
            {
                var time = times[i];
                var failureDate = new DateTime(
                    (int)time[2],
                    (int)time[1],
                    (int)time[0]
                );
                failures.Add(new Failure(
                    (FailureType)failureTypes[i],
                    failureDate,
                    deviceId[i]
                ));
            }

            var deviceList = devices
                .Select(d => new Device((int)d["DeviceId"], d["Name"] as string))
                .ToList();

            return FindDevicesFailedBeforeDate(date, failures, deviceList);
        }
    }
}