namespace GeneticAlgorithm.Core.Utils;

public interface IEvalWorkingSet
{
    void AddEval(double eval);

    bool IsStale();
}