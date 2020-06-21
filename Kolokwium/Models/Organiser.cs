using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kolokwium.Models
{
    public class Organiser
    {
        public int IdOrganizer { get; set; }
        public string Name { get; set; }

        public ICollection<EventOrganiser> EventOrganisers { get; set; }
    }
}
