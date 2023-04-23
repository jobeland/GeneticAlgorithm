using GeneticAlgorithms.Core.Evaluatable;
using GeneticAlgorithms.Core.Evolution;
using GeneticAlgorithms.Core.Utils;
using NeuralNetworks.Core.Factories;
using NeuralNetworks.Core.WeightInitializer;

namespace GeneticAlgorithms.Core;

public class GeneticAlgorithmFactory : IGeneticAlgorithmFactory
{
    private readonly IBreederFactory _breederFactory;
    private readonly IEvaluatableFactory _evaluatableFactory;
    private readonly IMutatorFactory _mutatorFactory;
    private readonly IEvalWorkingSetFactory _workingSetFactory;
    private readonly INeuralNetworkFactory _networkFactory;

    private GeneticAlgorithmFactory(INeuralNetworkFactory networkFactory, IEvalWorkingSetFactory workingSetFactory, IEvaluatableFactory evaluatableFactory, IBreederFactory breederFactory, IMutatorFactory mutatorFactory)
    {
        _networkFactory = networkFactory;
        _workingSetFactory = workingSetFactory;
        _evaluatableFactory = evaluatableFactory;
        _breederFactory = breederFactory;
        _mutatorFactory = mutatorFactory;
    }

    public static IGeneticAlgorithmFactory GetInstance(INeuralNetworkFactory networkFactory, IEvalWorkingSetFactory workingSetFactory, IEvaluatableFactory evaluatableFactory, IBreederFactory breederFactory, IMutatorFactory mutatorFactory)
    {
        return new GeneticAlgorithmFactory(networkFactory, workingSetFactory, evaluatableFactory, breederFactory, mutatorFactory);
    }

    public static IGeneticAlgorithmFactory GetInstance(IEvaluatableFactory evaluatableFactory)
    {
        NeuralNetworkFactory networkFactory = NeuralNetworkFactory.GetInstance();
        IEvalWorkingSetFactory workingSetFactory = EvalWorkingSetFactory.GetInstance();
        Random random = new();
        IBreederFactory breederFactory = BreederFactory.GetInstance(networkFactory, new RandomWeightInitializer(random));
        IMutatorFactory mutatorFactory = MutatorFactory.GetInstance(networkFactory, new RandomWeightInitializer(random));
        return new GeneticAlgorithmFactory(networkFactory, workingSetFactory, evaluatableFactory, breederFactory, mutatorFactory);
    }

    public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction epochAction)
    {
        return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory, epochAction);
    }

    public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig)
    {
        IBreeder breeder = _breederFactory.Create();
        IMutator mutator = _mutatorFactory.Create();
        IEvalWorkingSet workingSet = _workingSetFactory.Create();
        return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory, null);
    }

    public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig)
    {
        IBreeder breeder = _breederFactory.Create();
        IMutator mutator = _mutatorFactory.Create();
        IEvalWorkingSet workingSet = _workingSetFactory.Create();
        GenerationConfigurationSettings generationConfig = new()
        {
            GenerationPopulation = 100,
            UseMultithreading = true
        };
        EvolutionConfigurationSettings evolutionConfig = new()
        {
            GenerationsPerEpoch = 100,
            HighMutationRate = 0.5,
            NormalMutationRate = 0.05,
            NumEpochs = 10
        };
        return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory, null);
    }
}