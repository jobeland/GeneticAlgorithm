namespace GeneticAlgorithms.Core.Evolution;

public class EvolutionConfigurationSettings
{
    public int GenerationsPerEpoch { get; set; }
    public double HighMutationRate { get; set; }
    public double NormalMutationRate { get; set; }
    public int NumEpochs { get; set; }
    public int NumTopEvalsToReport { get; set; }
}