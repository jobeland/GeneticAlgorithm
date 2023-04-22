namespace GeneticAlgorithms.Core.Utils;

internal interface INeuralNetworkLoader
{
    INeuralNetwork LoadNeuralNetwork(string filename);
}