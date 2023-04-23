namespace GeneticAlgorithms.Core.Utils;

public class RandomGenerator
{
    private static readonly object padlock = new();

    private static Random? _random;

    public static Random GetInstance()
    {
        lock (padlock)
        {
            _random ??= new Random();
        }
        return _random; ;
    }
}