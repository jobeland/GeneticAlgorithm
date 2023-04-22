namespace GeneticAlgorithms.Core;

public class WeightedSession
{
    public double CumlativeWeight { get; set; }
    public ITrainingSession? Session { get; set; }
    public double Weight { get; set; }
}