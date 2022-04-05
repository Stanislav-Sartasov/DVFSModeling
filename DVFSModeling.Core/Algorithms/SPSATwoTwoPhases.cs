namespace DVFSModeling.Core.Algorithms
{
    public class SPSATwoTwoPhases : SPSAOne
    {

        const int UP_THRESHOLD = 64;

        const int FREQUENCY_PENALTY = 1000;

        Random r = new Random(DateTime.Now.Millisecond);

        bool finalPhase = false;

        double oldModel = 0;

        int oldDelta = 0;

        double ratio = 0;
        long betaUpscale = 0;

        double upscale;

        Dictionary<int, double> pows = new Dictionary<int, double>();

        public SPSATwoTwoPhases(int[] frequencies, long alpha, long beta, long upscale = 1) : base(frequencies, alpha, beta)
        {
            this.upscale = upscale;
            ratio = (double)alpha / beta;
            betaUpscale = beta * upscale;
            for (int i = 0; i < frequencies.Length; i++)
            {
                pows[i] = Math.Pow(1.5, i) * FREQUENCY_PENALTY;
            }
        }

        public override int NumberOfObservations => 2;

        public override int PredictFrequency(int currentFrequency, int currentLoad)
        {
            double model = CalculateFunctional(currentFrequency, currentLoad);

            if(finalPhase)
            {
                var result = currentFrequency - (model - oldModel) * ratio * oldDelta;

                finalPhase = false;
                return FindClosestFrequency(result);
            }
            else
            {
                finalPhase = true;
                oldModel = model;

                oldDelta = 2 * r.Next(2) - 1;

                var disturbedFrequency = FindClosestFrequency(currentFrequency + oldDelta * betaUpscale);
                return disturbedFrequency;
            }
        }

        private double CalculateFunctional(int currentFrequency, int currentLoad)
        {
            double model = 0;
            if (currentLoad > UP_THRESHOLD)
            {
                model = 2 << ((currentLoad - UP_THRESHOLD) / 2);
            }

            var index = indices[currentFrequency];

            model += pows[index];
            return model;
        }
    }
}
