using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVFSModeling.Core.Algorithms
{
    public class PowerSave : ADVFSAlgorithm
    {
        public PowerSave(int[] frequencies) : base(frequencies)
        {
        }

        public override int NumberOfObservations => 1;

        public override int PredictFrequency(int currentFrequency, int currentLoad)
        {
            return frequencies[0];
        }
    }
}
