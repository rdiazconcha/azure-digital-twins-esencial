using System;
using System.Collections.Generic;
using System.Text;

namespace Manager.Models
{
    class DeviceQuery
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string HardwareId { get; set; }
        public string SpaceId { get; set; }
        public string ConnectionString { get; set; }
    }
}
