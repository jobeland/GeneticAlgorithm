using System;
namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    public interface IEvalWorkingSet
    {
        void AddEval(double eval);
        bool IsStale();
    }
}
