using GeneticAlgorithms.Core.Evolution;

namespace GeneticAlgorithms.Core.Utils;

public interface IEpochAction
{
    ITrainingSession UpdateBestPerformer(IGeneration lastGenerationOfEpoch, int epochNumber);
}