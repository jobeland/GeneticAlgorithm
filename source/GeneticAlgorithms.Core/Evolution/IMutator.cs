namespace GeneticAlgorithms.Core.Evolution;

public interface IMutator
{
    INeuralNetwork Mutate(INeuralNetwork network, double mutateChance, out bool didMutate);

    IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance, out bool didMutate);
}