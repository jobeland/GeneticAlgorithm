using System;
namespace NeuralNetwork.GeneticAlgorithm
{
    public interface IGeneticAlgorithmFactory
    {
        IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig);
        IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, NeuralNetwork.GeneticAlgorithm.Evolution.GenerationConfigurationSettings generationConfig, NeuralNetwork.GeneticAlgorithm.Evolution.EvolutionConfigurationSettings evolutionConfig);
        IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, NeuralNetwork.GeneticAlgorithm.Evolution.GenerationConfigurationSettings generationConfig, NeuralNetwork.GeneticAlgorithm.Evolution.EvolutionConfigurationSettings evolutionConfig, ArtificialNeuralNetwork.Factories.INeuralNetworkFactory networkFactory, NeuralNetwork.GeneticAlgorithm.Evolution.IBreeder breeder, NeuralNetwork.GeneticAlgorithm.Evolution.IMutator mutator, NeuralNetwork.GeneticAlgorithm.Utils.IEvalWorkingSet workingSet, NeuralNetwork.GeneticAlgorithm.Evaluatable.IEvaluatableFactory evaluatableFactory);
    }
}
