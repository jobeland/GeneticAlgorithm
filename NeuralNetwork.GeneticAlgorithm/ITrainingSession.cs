using ArtificialNeuralNetwork;
using System;
namespace NeuralNetwork.GeneticAlgorithm
{
    public interface ITrainingSession
    {
        INeuralNetwork NeuralNet { get; }
        double GetSessionEvaluation();
        void Run();
    }
}
