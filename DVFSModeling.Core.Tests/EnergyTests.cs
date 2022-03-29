using DVFSModeling.Core.Algorithms;
using DVFSModeling.Core.Entities;
using DVFSModeling.Core.Helpers;
using NUnit.Framework;
using System;
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

        Dictionary<int, double> highWeights = new Dictionary<int, double>()
        {
            {774000, 56.85},
            {835000, 61.38},
            {919000, 70.65},
            {1002000, 79.53},
            {1085000, 91.11},
            {1169000, 105.19},
            {1308000, 130.33},
            {1419000, 152.46},
            {1530000, 177.39},
            {1670000, 209.73},
            {1733000, 233.56},
            {1796000, 247.53},
            {1860000, 269.61},
            {1923000, 291.52},
            {1986000, 307.98},
            {2050000, 324.33},
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPowerSave()
        {
            var algorithm = new PowerSave(weights.Keys.OrderBy(x => x).ToArray());

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
        public void TestSPSA2OnePhaseSimple()
        {
            var algorithm = new SPSATwoOnePhase(weights.Keys.OrderBy(x => x).ToArray(), 25, 62);

            var result = TestRunner.RunEnergyTest(algorithm, weights, Resources.flappy);

            Trace.WriteLine(result);
        }

        [Test]
        public void TestSPSA2OnePhaseSimplest()
        {
            var algorithm = new SPSATwoOnePhase(weights.Keys.OrderBy(x => x).ToArray(), 13, 86, 16000);
            var loads = new List<int>()
            {
                18, 44, 16, 48, 20, 45, 34
            };
            int fq = 2000000;
            foreach (var load in loads)
            {
                Trace.WriteLine(fq = algorithm.PredictFrequency(fq, load));
            }
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
        public void TestSPSATwoOnePhase()
        {
            var cases = new List<StampedLoad[]>()
            {
                //TraceLoader.Load(Resources.camera),
                TraceLoader.Load(Resources.flappy_new),
            };
            var o = new object();
            var keys = weights.Keys.OrderBy(x => x).ToArray();
            using (var fs = new FileStream("D:\\temp\\spsa2_one.csv", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                for (int beta = 1; beta <= 200; beta++)
                {
                    for (int upscale = 0; upscale <= 50000; upscale+=1000)
                    {
                        if (TestBetaUpscale(beta, upscale, keys))
                        {
                            Parallel.For(1, 201, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, alpha =>
                        {
                            int gcd = (int)Numerics.GCD(alpha, beta);

                            if (gcd > 1)
                            {
                                return;
                            }

                            var results = cases.Select(c =>
                            {
                                var algorithm = new SPSATwoOnePhase(weights.Keys.OrderBy(x => x).ToArray(), alpha, beta, upscale);

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
                                    sw.WriteLine($"{alpha};{beta};{upscale};{result.Energy};{result.TotalQuality};{result.QualityBetter};{result.QualityWorse}");
                                }
                        });
                        }
                    }
                }
            }
        }

        [Test]
        public void TestSPSATwoHighWeights()
        {
            var cases = new List<StampedLoad[]>()
            {
                TraceLoader.Load(Resources.flappy_new_high),
            };
            var o = new object();
            var keys = highWeights.Keys.OrderBy(x => x).ToArray();
            using (var fs = new FileStream("D:\\temp\\spsa2_one_high.csv", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                for (int beta = 1; beta <= 200; beta++)
                {
                    for (int upscale = 0; upscale <= 50000; upscale += 1000)
                    {
                        if (TestBetaUpscale(beta, upscale, keys))
                        {
                            Parallel.For(1, 201, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, alpha =>
                            {
                                int gcd = (int)Numerics.GCD(alpha, beta);

                                if (gcd > 1)
                                {
                                    return;
                                }

                                var results = cases.Select(c =>
                                {
                                    var algorithm = new SPSATwoOnePhase(highWeights.Keys.OrderBy(x => x).ToArray(), alpha, beta, upscale);

                                    return TestRunner.RunEnergyTest(algorithm, highWeights, c);
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
                                        sw.WriteLine($"{alpha};{beta};{upscale};{result.Energy};{result.TotalQuality};{result.QualityBetter};{result.QualityWorse}");
                                    }
                            });
                        }
                    }
                }
            }
        }

        [Test]
        public void TestSPSATwoTwoPhases()
        {
            var cases = new List<StampedLoad[]>()
            {
                //TraceLoader.Load(Resources.camera),
                TraceLoader.Load(Resources.flappy_new),
            };
            var o = new object();
            var keys = weights.Keys.OrderBy(x => x).ToArray();
            using (var fs = new FileStream("D:\\temp\\spsa2_two.csv", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                for (int beta = 1; beta <= 200; beta++)
                {
                    for (int upscale = 0; upscale <= 50000; upscale += 1000)
                    {
                        if (TestBetaUpscale(beta, upscale, keys))
                        {
                            Parallel.For(1, 201, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, alpha =>
                            {
                                int gcd = (int)Numerics.GCD(alpha, beta);

                                if (gcd > 1)
                                {
                                    return;
                                }

                                var results = cases.Select(c =>
                                {
                                    var algorithm = new SPSATwoTwoPhases(weights.Keys.OrderBy(x => x).ToArray(), alpha, beta, upscale);

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
                                        sw.WriteLine($"{alpha};{beta};{upscale};{result.Energy};{result.TotalQuality};{result.QualityBetter};{result.QualityWorse}");
                                    }
                            });
                        }
                    }
                }
            }
        }

        [Test]
        public void TestSPSATwoTwoPhasesHighWeights()
        {
            var cases = new List<StampedLoad[]>()
            {
                TraceLoader.Load(Resources.flappy_new_high),
            };
            var o = new object();
            var keys = highWeights.Keys.OrderBy(x => x).ToArray();
            using (var fs = new FileStream("D:\\temp\\spsa2_two_high.csv", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                for (int beta = 1; beta <= 200; beta++)
                {
                    for (int upscale = 0; upscale <= 50000; upscale += 1000)
                    {
                        if (TestBetaUpscale(beta, upscale, keys))
                        {
                            Parallel.For(1, 201, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, alpha =>
                            {
                                int gcd = (int)Numerics.GCD(alpha, beta);

                                if (gcd > 1)
                                {
                                    return;
                                }

                                var results = cases.Select(c =>
                                {
                                    var algorithm = new SPSATwoTwoPhases(highWeights.Keys.OrderBy(x => x).ToArray(), alpha, beta, upscale);

                                    return TestRunner.RunEnergyTest(algorithm, highWeights, c);
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
                                        sw.WriteLine($"{alpha};{beta};{upscale};{result.Energy};{result.TotalQuality};{result.QualityBetter};{result.QualityWorse}");
                                    }
                            });
                        }
                    }
                }
            }
        }

        private bool TestBetaUpscale(int beta, int upscale, int[] frequencies)
        {
            for(int i=0;i< frequencies.Length;i++)
            {
                var src = frequencies[i];
                var target = FindClosestFrequency(frequencies[i] - beta * upscale, frequencies);
                if (target != src) 
                    return true;
                target = FindClosestFrequency(frequencies[i] + beta * upscale, frequencies);
                if (target != src) 
                    return true;
            }
            return false;
        }

        private int FindClosestFrequency(double result, int[] frequencies)
        {
            // projecting into frequencies set
            var closest = 0;
            var diff = double.MaxValue;
            for (int i = 0; i < frequencies.Length; i++)
            {
                var curDiff = Math.Abs(result - frequencies[i]);
                if (curDiff < diff)
                {
                    diff = curDiff;
                    closest = frequencies[i];
                }
            }

            return closest;
        }
    }
}