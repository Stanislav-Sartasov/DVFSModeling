using DVFSModeling.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVFSModeling.Core.Helpers
{
    public static class TraceLoader
    {
        public static StampedLoad[] Load(string traceCsv)
        {
            return traceCsv
                .Trim()
                .Split(Environment.NewLine)
                .Select(s =>
                {
                    var parts = s.Split(';');
                    return new StampedLoad()
                    {
                        Load = int.Parse(parts[2]),
                        OriginalFrequency = int.Parse(parts[1]),
                        Timestamp = double.Parse(parts[0]),
                    }; 
                }).ToArray();
        }
    }
}
