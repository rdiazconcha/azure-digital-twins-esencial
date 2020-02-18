using System;
using System.Collections.Generic;

namespace Manager.Models
{
    class SpaceQuery
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<SpaceValueQuery> Values { get; set; }
    }
}
