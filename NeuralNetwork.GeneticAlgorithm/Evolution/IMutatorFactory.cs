using System;
namespace NeuralNetwork.GeneticAlgorithm.Evolution
{
    public interface IMutatorFactory
    {
        IMutator Create(MutationConfigurationSettings config);
        IMutator Create();
    }
}
