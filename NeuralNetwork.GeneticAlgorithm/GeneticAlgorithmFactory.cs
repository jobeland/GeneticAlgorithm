using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.WeightInitializer;
using NeuralNetwork.GeneticAlgorithm.Evaluatable;
using NeuralNetwork.GeneticAlgorithm.Evolution;
using NeuralNetwork.GeneticAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.GeneticAlgorithm
{
    public class GeneticAlgorithmFactory : IGeneticAlgorithmFactory
    {
        private INeuralNetworkFactory _networkFactory;
        private readonly IEvalWorkingSetFactory _workingSetFactory;
        private readonly IEvaluatableFactory _evaluatableFactory;
        private readonly IBreederFactory _breederFactory;
        private readonly IMutatorFactory _mutatorFactory;
        private readonly INeuralNetworkSaver _neuralNetworkSaver;

        private GeneticAlgorithmFactory(INeuralNetworkFactory networkFactory, IEvalWorkingSetFactory workingSetFactory, IEvaluatableFactory evaluatableFactory, IBreederFactory breederFactory, IMutatorFactory mutatorFactory, INeuralNetworkSaver neuralNetworkSaver)
        {
            _networkFactory = networkFactory;
            _workingSetFactory = workingSetFactory;
            _evaluatableFactory = evaluatableFactory;
            _breederFactory = breederFactory;
            _mutatorFactory = mutatorFactory;
            _neuralNetworkSaver = neuralNetworkSaver;
        }

        public static IGeneticAlgorithmFactory GetInstance(INeuralNetworkFactory networkFactory, IEvalWorkingSetFactory workingSetFactory, IEvaluatableFactory evaluatableFactory, IBreederFactory breederFactory, IMutatorFactory mutatorFactory, INeuralNetworkSaver neuralNetworkSaver)
        {
            return new GeneticAlgorithmFactory(networkFactory, workingSetFactory, evaluatableFactory, breederFactory, mutatorFactory, neuralNetworkSaver);
        }

        public static IGeneticAlgorithmFactory GetInstance(IEvaluatableFactory evaluatableFactory)
        {
            var networkFactory = NeuralNetworkFactory.GetInstance();
            var workingSetFactory = EvalWorkingSetFactory.GetInstance();
            var random = new Random();
            var breederFactory = BreederFactory.GetInstance(networkFactory, new RandomWeightInitializer(random));
            var mutatorFactory = MutatorFactory.GetInstance(networkFactory, new RandomWeightInitializer(random));
            var networkSaver = new NeuralNetworkSaver("\\networks");
            return new GeneticAlgorithmFactory(networkFactory, workingSetFactory, evaluatableFactory, breederFactory, mutatorFactory, networkSaver);
        }

        public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction epochAction, INeuralNetworkSaver neuralNetworkSaver)
        {
            return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, networkFactory, breeder, mutator, workingSet, evaluatableFactory, epochAction, neuralNetworkSaver);
        }

        public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig)
        {
            var breeder = _breederFactory.Create();
            var mutator = _mutatorFactory.Create();
            var workingSet = _workingSetFactory.Create();
            var saver = _neuralNetworkSaver ?? new NeuralNetworkSaver("\\networks");
            return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory, null, saver);
        }

        public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig)
        {
            var breeder = _breederFactory.Create();
            var mutator = _mutatorFactory.Create();
            var workingSet = _workingSetFactory.Create();
            var generationConfig = new GenerationConfigurationSettings
            {
                GenerationPopulation = 100,
                UseMultithreading = true
            };
            var evolutionConfig = new EvolutionConfigurationSettings
            {
                GenerationsPerEpoch = 100,
                HighMutationRate = 0.5,
                NormalMutationRate = 0.05,
                NumEpochs = 10
            };
            var saver = _neuralNetworkSaver ?? new NeuralNetworkSaver("\\networks");
            return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory, null, saver);
        }

    }
}
