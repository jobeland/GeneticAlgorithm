using System;
namespace NeuralNetwork.GeneticAlgorithm.Evolution
{
    public interface IBreederFactory
    {
        IBreeder Create(double motherFatherBias = 0.5);
    }
}
