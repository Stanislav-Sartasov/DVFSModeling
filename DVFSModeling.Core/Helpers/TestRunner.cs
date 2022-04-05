using DVFSModeling.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVFSModeling.Core.Helpers
{
    static public class TestRunner
    {
        public static int NumberOfTests { get; set; } = 10;

        public static EnergyTestResult RunEnergyTest(IDVFSAlgorithm algorithm, Dictionary<int, double> weights, string trace)
        {
            return RunEnergyTest(algorithm, weights, TraceLoader.Load(trace));
        }

        public static EnergyTestResult RunEnergyTest(IDVFSAlgorithm algorithm, Dictionary<int, double> weights, StampedLoad[] trace)
        {
            List<double> energies = new List<double>(NumberOfTests);
            List<int> qualitiesWorse = new List<int>(NumberOfTests);
            List<int> qualitiesBetter = new List<int>(NumberOfTests);

            for (int i = 0; i < NumberOfTests; i++)
            {
                int currentFrequency = trace.First().OriginalFrequency;
                double previousTime = trace.First().Timestamp;

                double result = 0;
                int qualityWorse = 0;
                int qualityBetter = 0;

                foreach (var entry in trace)
                {
                    var currentLoad = (int)Math.Min(100.0, (double)entry.Load * entry.OriginalFrequency / currentFrequency);

                    if (entry.Load != 100 && currentLoad == 100) qualityWorse++;
                    if (entry.Load == 100 && currentLoad != 100) qualityBetter++;

                    result += (currentLoad / 100.0d) * weights[currentFrequency] * (entry.Timestamp - previousTime);

                    currentFrequency = algorithm.PredictFrequency(currentFrequency, currentLoad);

                    previousTime = entry.Timestamp;
                }

                energies.Add(result);
                qualitiesWorse.Add(qualityWorse);
                qualitiesBetter.Add(qualityBetter);
            }

            return new EnergyTestResult() { Energy = energies.Average(),
                TotalQuality = (int)(qualitiesBetter.Average() - qualitiesWorse.Average()),
                QualityBetter = (int)qualitiesBetter.Average(),
                QualityWorse = (int)qualitiesWorse.Average(),
            };
        }
    }
}
