using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Data
{
    public class Timer
    {
        public int timerID { get; set; }
        public string timerName { get; set; }
        public double duration { get; set; }
        public DateTime startTime { get; set; }

    }
}
