using System;
using System.Collections.Generic;
namespace NeuralNetwork.GeneticAlgorithm.Evolution
{
    public interface IGeneration
    {
        ITrainingSession GetBestPerformer();
        IList<ITrainingSession> GetBestPerformers(int numPerformers);
        double[] GetEvalsForGeneration();
        void Run();
    }
}
