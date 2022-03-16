using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVFSModeling.Core.Helpers
{
    public static class Numerics
    {
        static public long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
