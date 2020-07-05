using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumPopr.Models
{
    public class FiretruckAction
    {
        public int IdFiretruckAction { get; set; }
        public int IdFireTruck { get; set; }
        public int IdAction { get; set; }
        public DateTime AssignmentDate { get; set; }


        public Action Action { get; set; }
        public Firetruck Firetruck { get; set; }
    }
}
