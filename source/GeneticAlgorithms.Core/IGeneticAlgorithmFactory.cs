using GeneticAlgorithms.Core.Evaluatable;
using GeneticAlgorithms.Core.Evolution;
using GeneticAlgorithms.Core.Utils;
using NeuralNetworks.Core.Factories;

namespace GeneticAlgorithms.Core;

public interface IGeneticAlgorithmFactory
{
    IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig);

    IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig);

    IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction epochAction);
}