using ArtificialNeuralNetwork.Factories;
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

        public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory)
        {
            return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory);
        }

        public IGeneticAlgorithm Create(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig)
        {
            var breeder = _breederFactory.Create();
            var mutator = _mutatorFactory.Create();
            var workingSet = _workingSetFactory.Create();
            return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory);
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
            return GeneticAlgorithm.GetInstance(networkConfig, generationConfig, evolutionConfig, _networkFactory, breeder, mutator, workingSet, _evaluatableFactory);
        }

    }
}
