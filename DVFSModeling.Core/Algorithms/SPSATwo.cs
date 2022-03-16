namespace DVFSModeling.Core.Algorithms
{
    public class SPSATwo : SPSAOne
    {

        const int UP_THRESHOLD = 64;

        const int FREQUENCY_PENALTY = 1000;

        Random r = new Random(DateTime.Now.Millisecond);

        bool finalPhase = false;

        double oldModel = 0;

        public SPSATwo(int[] frequencies, long alpha, long beta) : base(frequencies, alpha, beta)
        {
        }

        public override int NumberOfObservations => 2;

        public override int PredictFrequency(int currentFrequency, int currentLoad)
        {
            double model = 0;
            if (currentLoad > UP_THRESHOLD)
            {
                model = 2 << ((currentLoad - UP_THRESHOLD) / 2);
            }

            var index = indices[currentFrequency];

            model += Math.Pow(1.5, index) * FREQUENCY_PENALTY;

            if(finalPhase)
            {
                var delta = 2 * r.Next(2) - 1;

                var result = currentFrequency + (model - oldModel) / beta * alpha * delta;

                finalPhase = false;
                return FindClosestFrequency(result);
            }
            else
            {
                finalPhase = true;
                oldModel = model;
                return currentFrequency;
            }
        }
    }
}
