namespace GeneticAlgorithms.Core.Evolution;

public interface IGeneration
{
    ITrainingSession GetBestPerformer();

    IList<ITrainingSession> GetBestPerformers(int numPerformers);

    double[] GetEvalsForGeneration();

    void Run();
}