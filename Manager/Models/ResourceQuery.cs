using System;

namespace Manager.Models
{
    class ResourceQuery
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string SpaceId { get; set; }
    }
}
