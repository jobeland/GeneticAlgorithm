using System;
namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    public interface INeuralNetworkSaver
    {
        string SaveNeuralNetwork(ArtificialNeuralNetwork.INeuralNetwork network, double networkEvaluation, int epoch);
    }
}
