namespace GeneticAlgorithms.Core.Utils;

public class EvalWorkingSet : IEvalWorkingSet
{
    private readonly LinkedList<Double> _pastEvals;
    private readonly int _size;

    private EvalWorkingSet(int size)
    {
        _pastEvals = new LinkedList<Double>();
        _pastEvals.AddFirst(0.0);
        this._size = size;
    }

    public static IEvalWorkingSet GetInstance(int size)
    {
        return new EvalWorkingSet(size);
    }

    public void AddEval(double eval)
    {
        _pastEvals.AddFirst(eval);
        if (_pastEvals.Count > this._size)
        {
            _pastEvals.RemoveLast();
        }
    }

    public bool IsStale()
    {
        if (_pastEvals.First?.Value <= _pastEvals.Last?.Value)
        {
            return true;
        }
        return false;
    }
}