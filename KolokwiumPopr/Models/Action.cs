using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumPopr.Models
{
    public class Action
    {
        public int IdAction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool NeedSpecialEquipment { get; set; }


        public ICollection<FirefighterAction> FirefighterActions { get; set; }
        public ICollection<FiretruckAction> FiretruckActions { get; set; }
}
}
