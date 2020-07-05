using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumPopr.DTOs.Requests
{
    public class AssignFiretruckRequest
    {
        public int IdAction { get; set; }
        public int IdFireTruck { get; set; }
    }
}
