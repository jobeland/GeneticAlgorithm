using GeneticAlgorithm.Core.Evaluatable;
using GeneticAlgorithm.Core.Evolution;
using GeneticAlgorithm.Core.Utils;
using NeuralNetwork.Core.Factories;

namespace GeneticAlgorithm.Core;

public interface IGeneticAlgorithmFactory
{
    IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig);

    IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig);

    IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction epochAction);
}