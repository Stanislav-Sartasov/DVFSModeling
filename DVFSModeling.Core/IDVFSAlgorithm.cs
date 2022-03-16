namespace DVFSModeling.Core
{
    public interface IDVFSAlgorithm
    {
        int NumberOfObservations { get; }
        int PredictFrequency(int currentFrequency, int currentLoad);
    }
}