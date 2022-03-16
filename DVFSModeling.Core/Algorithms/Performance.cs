namespace DVFSModeling.Core.Algorithms
{
    public class Performance : ADVFSAlgorithm
    {
        public Performance(int[] frequencies) : base(frequencies)
        {
        }

        public override int NumberOfObservations => 1;

        public override int PredictFrequency(int currentFrequency, int currentLoad)
        {
            return frequencies[frequencies.Length - 1];
        }
    }
}
