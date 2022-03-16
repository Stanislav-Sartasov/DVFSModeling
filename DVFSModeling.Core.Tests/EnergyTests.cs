using DVFSModeling.Core.Algorithms;
using DVFSModeling.Core.Entities;
using DVFSModeling.Core.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DVFSModeling.Core.Tests
{
    public class EnergyTests
    {
        Dictionary<int, double> weights = new Dictionary<int, double>()
        {
            { 500000, 19.55 },
            { 774000, 23.5 },
            { 875000, 25 },
            { 975000, 27.86 },
            { 1075000, 31.24 },
            { 1175000, 35.5 },
            { 1275000, 39.69 },
            { 1375000, 44.83 },
            { 1500000, 52.33 },
            { 1618000, 58.95 },
            { 1666000, 62.05 },
            { 1733000, 66.61 },
            { 1800000, 72.77 },
            { 1866000, 80.27 },
            { 1933000, 85.8 },
            { 2000000, 90.04 },
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPowerSave()
        {
            var algorithm = new PowerSave(weights.Keys.OrderBy(x=>x).ToArray());

            var result = TestRunner.RunEnergyTest(algorithm, weights, Resources.camera);

            Trace.WriteLine(result);
        }

        [Test]
        public void TestPerformance()
        {
            var algorithm = new Performance(weights.Keys.OrderBy(x => x).ToArray());

            var result = TestRunner.RunEnergyTest(algorithm, weights, Resources.camera);

            Trace.WriteLine(result);
        }

        [Test]
        public void TestSPSA2Simple()
        {
            var algorithm = new SPSATwo(weights.Keys.OrderBy(x => x).ToArray(), 4, 1);

            var result = TestRunner.RunEnergyTest(algorithm, weights, Resources.flappy);

            Trace.WriteLine(result);
        }

        [Test]
        public void TestSPSA1Simple()
        {
            var algorithm = new SPSAOne(weights.Keys.OrderBy(x => x).ToArray(), 4, 1);

            var result = TestRunner.RunEnergyTest(algorithm, weights, Resources.camera);

            Trace.WriteLine(result);
        }

        [Test]
        public void TestSPSAOne()
        {
            var cases = new List<StampedLoad[]>()
            {
                TraceLoader.Load(Resources.camera),
                TraceLoader.Load(Resources.flappy),
            };
            var o = new object();
            using (var fs = new FileStream("D:\\temp\\spsa1.csv", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                for (int alpha = 1; alpha <= 100; alpha++)
                {
                    Parallel.For(1, 101, new ParallelOptions() { MaxDegreeOfParallelism = 6 }, beta =>
                     {
                         int gcd = (int)Numerics.GCD(alpha, beta);

                         if (gcd > 1)
                         {
                             return;
                         }

                         var results = cases.Select(c =>
                         {
                             var algorithm = new SPSAOne(weights.Keys.OrderBy(x => x).ToArray(), alpha, beta);

                             return TestRunner.RunEnergyTest(algorithm, weights, c);
                         }).ToList();

                         var result = new EnergyTestResult()
                         {
                             Energy = results.Sum(x => x.Energy),
                             TotalQuality = results.Sum(x => x.TotalQuality),
                             QualityBetter = results.Sum(x => x.QualityBetter),
                             QualityWorse = results.Sum(x => x.QualityWorse),
                         };

                         if (result.TotalQuality > 0)
                             lock (o)
                             {
                                 sw.WriteLine($"{alpha};{beta};{result.Energy};{result.TotalQuality};{result.QualityBetter};{result.QualityWorse}");
                             }
                     });
                }
            }
        }

        [Test]
        public void TestSPSATwo()
        {
            var cases = new List<StampedLoad[]>()
            {
                TraceLoader.Load(Resources.camera),
                TraceLoader.Load(Resources.flappy),
            };
            var o = new object();
            using (var fs = new FileStream("D:\\temp\\spsa2.csv", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                for (int alpha = 1; alpha <= 100; alpha++)
                {
                    Parallel.For(1, 101, new ParallelOptions() { MaxDegreeOfParallelism = 6 }, beta =>
                    {
                        int gcd = (int)Numerics.GCD(alpha, beta);

                        if (gcd > 1)
                        {
                            return;
                        }

                        var results = cases.Select(c =>
                        {
                            var algorithm = new SPSATwo(weights.Keys.OrderBy(x => x).ToArray(), alpha, beta);

                            return TestRunner.RunEnergyTest(algorithm, weights, c);
                        }).ToList();

                        var result = new EnergyTestResult()
                        {
                            Energy = results.Sum(x => x.Energy),
                            TotalQuality = results.Sum(x => x.TotalQuality),
                            QualityBetter = results.Sum(x => x.QualityBetter),
                            QualityWorse = results.Sum(x => x.QualityWorse),
                        };

                        if (result.TotalQuality > 0)
                            lock (o)
                            {
                                sw.WriteLine($"{alpha};{beta};{result.Energy};{result.TotalQuality};{result.QualityBetter};{result.QualityWorse}");
                            }
                    });
                }
            }
        }
    }
}