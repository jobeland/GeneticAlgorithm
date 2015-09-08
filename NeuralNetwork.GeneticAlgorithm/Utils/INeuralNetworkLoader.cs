using ArtificialNeuralNetwork;
using System;
namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    interface INeuralNetworkLoader
    {
        INeuralNetwork LoadNeuralNetwork(string filename);
    }
}
