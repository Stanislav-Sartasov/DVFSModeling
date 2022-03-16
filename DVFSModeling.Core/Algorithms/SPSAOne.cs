using DVFSModeling.Core.Helpers;

namespace DVFSModeling.Core.Algorithms
{
    public class SPSAOne : ADVFSAlgorithm
    {

        const int UP_THRESHOLD = 64;

        const int FREQUENCY_PENALTY = 1000;

        protected long alpha = 0;

        protected long beta = 0;

        protected Dictionary<int, int> indices = new Dictionary<int, int>();

        Random r = new Random(DateTime.Now.Millisecond);

        public SPSAOne(int[] frequencies, long alpha, long beta) : base(frequencies)
        {
            var gcd = Numerics.GCD(alpha, beta);
            this.alpha = alpha / gcd;
            this.beta = beta / gcd;

            for (int i = 0; i < frequencies.Length; i++)
            {
                indices[frequencies[i]] = i;
            }
        }

        public override int NumberOfObservations => 1;

        public override int PredictFrequency(int currentFrequency, int currentLoad)
        {
            double step = 0;
            if (currentLoad > UP_THRESHOLD)
            {
                step = 2 << ((currentLoad - UP_THRESHOLD) / 2);
            }

            var index = indices[currentFrequency];

            step += Math.Pow(1.5, index) * FREQUENCY_PENALTY;

            var delta = 2 * r.Next(2) - 1;

            var result = currentFrequency + step / beta * alpha * delta;
            return FindClosestFrequency(result);
        }

        protected int FindClosestFrequency(double result)
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
