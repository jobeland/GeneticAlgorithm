namespace GeneticAlgorithms.Core;

public interface IGeneticAlgorithm
{
    INeuralNetwork GetBestPerformer();

    void RunSimulation();
}