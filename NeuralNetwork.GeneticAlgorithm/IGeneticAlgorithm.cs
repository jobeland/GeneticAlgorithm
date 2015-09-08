using ArtificialNeuralNetwork;
using System;
namespace NeuralNetwork.GeneticAlgorithm
{
    public interface IGeneticAlgorithm
    {
        INeuralNetwork GetBestPerformer();
        void RunSimulation();
    }
}
