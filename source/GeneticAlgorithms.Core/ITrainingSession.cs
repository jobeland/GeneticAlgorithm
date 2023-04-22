namespace GeneticAlgorithms.Core;

public interface ITrainingSession
{
    INeuralNetwork NeuralNet { get; }

    double GetSessionEvaluation();

    void Run();
}