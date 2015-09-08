using System;
namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    interface INeuralNetworkSaver
    {
        string SaveNeuralNetwork(ArtificialNeuralNetwork.INeuralNetwork network, double networkEvaluation, int epoch);
    }
}
