namespace GeneticAlgorithms.Core.Evolution;

public interface IBreederFactory
{
    IBreeder Create(double motherFatherBias = 0.5);
}