namespace DVFSModeling.Core
{

    public abstract class ADVFSAlgorithm:IDVFSAlgorithm
    {
        protected int[] frequencies;
        public ADVFSAlgorithm(int[] frequencies)
        {
            this.frequencies = frequencies;
        }

        public abstract int NumberOfObservations { get; }

        public abstract int PredictFrequency(int currentFrequency, int currentLoad);
    }
}