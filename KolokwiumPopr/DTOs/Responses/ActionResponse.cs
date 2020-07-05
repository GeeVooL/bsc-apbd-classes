using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumPopr.DTOs.Responses
{
    public class ActionResponse
    {
        public IEnumerable<ActionResponseItem> Actions { get; set; }
    }
}
