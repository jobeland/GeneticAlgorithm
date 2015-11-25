using NeuralNetwork.GeneticAlgorithm.Evolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    public interface IEpochAction
    {
        ITrainingSession UpdateBestPerformer(IGeneration lastGenerationOfEpoch, int epochNumber);
    }
}
