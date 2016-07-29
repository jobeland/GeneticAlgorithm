namespace NeuralNetwork.GeneticAlgorithm.Evolution
{
    public class EvolutionConfigurationSettings
    {
        public double NormalMutationRate { get; set; }
        public double HighMutationRate { get; set; }
        public int GenerationsPerEpoch { get; set; }
        public int NumEpochs { get; set; }
        public int NumTopEvalsToReport { get; set; }
    }
}
