using NeuralNetworks.Core.ActivationFunctions;

namespace GeneticAlgorithms.Core;

public class NeuralNetworkConfigurationSettings
{
    public IActivationFunction? ActivationFunction { get; set; }
    public int NumHiddenLayers { get; set; }
    public int NumHiddenNeurons { get; set; }
    public int NumInputNeurons { get; set; }
    public int NumOutputNeurons { get; set; }
    public ISummationFunction? SummationFunction { get; set; }
}