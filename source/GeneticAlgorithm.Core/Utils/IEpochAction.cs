using GeneticAlgorithm.Core.Evolution;

namespace GeneticAlgorithm.Core.Utils;

public interface IEpochAction
{
    ITrainingSession UpdateBestPerformer(IGeneration lastGenerationOfEpoch, int epochNumber);
}