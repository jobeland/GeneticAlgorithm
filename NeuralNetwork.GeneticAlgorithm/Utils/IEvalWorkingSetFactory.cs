using System;
namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    public interface IEvalWorkingSetFactory
    {
        IEvalWorkingSet Create(int size = 50);
    }
}
