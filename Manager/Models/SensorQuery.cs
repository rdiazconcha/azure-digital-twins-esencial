using System;

namespace Manager.Models
{
    class SensorQuery
    {
        public Guid Id { get; set; }
        public string DataType { get; set; }
        public string HardwareId { get; set; }
    }
}
