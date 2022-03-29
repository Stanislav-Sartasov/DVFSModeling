namespace DVFSModeling.Core.Algorithms
{
    public class SPSATwoOnePhase : SPSAOne
    {

        const int UP_THRESHOLD = 64;

        const int FREQUENCY_PENALTY = 1000;

        Random r = new Random(DateTime.Now.Millisecond);

        double upscale;

        public SPSATwoOnePhase(int[] frequencies, long alpha, long beta, long upscale = 1) : base(frequencies, alpha, beta)
        {
            this.upscale = upscale;
        }

        public override int NumberOfObservations => 2;

        public override int PredictFrequency(int currentFrequency, int currentLoad)
        {
            double undisturbedModel = CalculateFunctional(currentFrequency, currentLoad);

            var deltaForFreq = 2 * r.Next(2) - 1;

            var disturbedFrequency = FindClosestFrequency(currentFrequency + deltaForFreq * upscale * beta);
            var disturbedLoad = currentLoad * disturbedFrequency / currentFrequency;

            double disturbedModel =
                CalculateFunctional(
                    FindClosestFrequency(currentFrequency + deltaForFreq * upscale * beta), disturbedLoad);

            var delta = 2 * r.Next(2) - 1;

            var result = currentFrequency + (undisturbedModel - disturbedModel) / beta * alpha * delta;

            return FindClosestFrequency(result);
        }

        private double CalculateFunctional(int currentFrequency, int currentLoad)
        {
            double model = 0;
            if (currentLoad > UP_THRESHOLD)
            {
                model = 2 << ((currentLoad - UP_THRESHOLD) / 2);
            }

            var index = indices[currentFrequency];

            model += Math.Pow(1.5, index) * FREQUENCY_PENALTY;
            return model;
        }
    }
}
