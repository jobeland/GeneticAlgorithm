namespace GeneticAlgorithm.Core.Utils;

public interface IEvalWorkingSetFactory
{
    IEvalWorkingSet Create(int size = 50);
}