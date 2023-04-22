namespace GeneticAlgorithms.Core.Evolution;

public class MutationConfigurationSettings
{
    public bool MutateAxonActivationFunction { get; set; }
    public bool MutateNumberOfHiddenLayers { get; set; }
    public bool MutateNumberOfHiddenNeuronsInLayer { get; set; }
    public bool MutateSomaBiasFunction { get; set; }
    public bool MutateSomaSummationFunction { get; set; }
    public bool MutateSynapseWeights { get; set; }
}