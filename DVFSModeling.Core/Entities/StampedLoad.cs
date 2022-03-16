using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVFSModeling.Core.Entities
{
    public class StampedLoad
    {
        public double Timestamp { get; set; }
        public int OriginalFrequency { get; set; }
        public int Load { get; set; }
    }
}
