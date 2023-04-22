namespace GeneticAlgorithms.Core.Utils;

public class EvalWorkingSetFactory : IEvalWorkingSetFactory
{
    private EvalWorkingSetFactory()
    { }

    public static IEvalWorkingSetFactory GetInstance()
    {
        return new EvalWorkingSetFactory();
    }

    public IEvalWorkingSet Create(int size = 50)
    {
        return EvalWorkingSet.GetInstance(size);
    }
}