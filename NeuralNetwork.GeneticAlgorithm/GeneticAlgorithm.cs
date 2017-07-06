using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.Genes;
using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeuralNetwork.GeneticAlgorithm.Evaluatable;
using NeuralNetwork.GeneticAlgorithm.Evolution;
using NeuralNetwork.GeneticAlgorithm.Utils;

namespace NeuralNetwork.GeneticAlgorithm
{

    public class GeneticAlgorithm : IGeneticAlgorithm
    {
        private INeuralNetworkFactory _networkFactory;
        private readonly IEvalWorkingSet _history;
        private readonly IEvaluatableFactory _evaluatableFactory;

        private readonly NeuralNetworkConfigurationSettings _networkConfig;
        private readonly GenerationConfigurationSettings _generationConfig;
        private readonly EvolutionConfigurationSettings _evolutionConfig;

        private readonly IBreeder _breeder;
        private readonly IMutator _mutator;

        private IGeneration _generation;
        private ITrainingSession _bestPerformerOfEpoch;
        private IEpochAction _epochAction;

        private GeneticAlgorithm(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction epochAction)
        {
            _networkConfig = networkConfig;
            _generationConfig = generationConfig;
            _evolutionConfig = evolutionConfig;
            _epochAction = epochAction;
            var sessions = new List<ITrainingSession>();
            _networkFactory = networkFactory;
            _breeder = breeder;
            _mutator = mutator;
            _history = workingSet;
            _evaluatableFactory = evaluatableFactory;
            for (int i = 0; i < _generationConfig.GenerationPopulation; i++)
            {
                var network = _networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, _networkConfig.NumHiddenLayers, _networkConfig.NumHiddenNeurons);
                sessions.Add(new TrainingSession(network, _evaluatableFactory.Create(network), i));
            }
            _generation = new Generation(sessions, _generationConfig);
        }

        public static IGeneticAlgorithm GetInstance(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction epochAction)
        {
            return new GeneticAlgorithm(networkConfig, generationConfig, evolutionConfig, networkFactory, breeder, mutator, workingSet, evaluatableFactory, epochAction);
        }

        public void RunSimulation()
        {
            RunStarterGeneration();
            for (int epoch = 1; epoch < _evolutionConfig.NumEpochs; epoch++)
            {
                for (int generation = 0; generation < _evolutionConfig.GenerationsPerEpoch; generation++)
                {
                    if (generation == 0)
                    {
                        LoggerFactory.GetLogger().Log(LogLevel.Info, $"Creating next generation with top performer with eval: {_bestPerformerOfEpoch.GetSessionEvaluation()}");
                        createNextGeneration(_bestPerformerOfEpoch);
                    }
                    else
                    {
                        createNextGeneration(null);
                    }
                    _generation.Run();

                    IEnumerable<double> evals = _generation.GetEvalsForGeneration().OrderByDescending(d => d);
                    var evalsTotalCount = evals.Count();
                    if (_evolutionConfig.NumTopEvalsToReport > 0)
                    {
                        evals = evals.Take(_evolutionConfig.NumTopEvalsToReport);
                    }

                    var sb = new StringBuilder();
                    foreach (var t in evals.OrderBy(d => d))
                    {
                        sb.AppendLine($"eval: {t}");
                    }
                    LoggerFactory.GetLogger().Log(LogLevel.Info, sb.ToString());
                    LoggerFactory.GetLogger().Log(LogLevel.Info, $"count: {evalsTotalCount}");
                    LoggerFactory.GetLogger().Log(LogLevel.Info, $"Epoch: {epoch},  Generation: {generation}");

                }
                if (_epochAction != null)
                {
                    LoggerFactory.GetLogger().Log(LogLevel.Info, "Updating best performer");
                    _bestPerformerOfEpoch = _epochAction.UpdateBestPerformer(_generation, epoch);
                }
                else
                {
                    _bestPerformerOfEpoch = GetBestPerformerOfGeneration();
                }
                SaveBestPerformer(epoch);
            }
        }

        internal void RunStarterGeneration(){
            _generation.Run();
            if (_epochAction != null)
            {
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Getting best performer");
                _bestPerformerOfEpoch = _epochAction.UpdateBestPerformer(_generation, 0);
            }
            else
            {
                _bestPerformerOfEpoch = GetBestPerformerOfGeneration();
            }
        }

        public INeuralNetwork GetBestPerformer()
        {
            if (_bestPerformerOfEpoch == null)
            {
                throw new InvalidOperationException("Cannot return best performer before simulation is complete");
            }
            return _bestPerformerOfEpoch.NeuralNet;
        }

        internal ITrainingSession GetBestPerformerOfGeneration()
        {
            if (_generation == null)
            {
                throw new InvalidOperationException("Cannot return best performer before generation is simulated");
            }
            return _generation.GetBestPerformer();
        }

        internal void SaveBestPerformer(int epoch)
        {
            ITrainingSession bestPerformer = _generation.GetBestPerformer();
            var saver = new NeuralNetworkSaver("\\networks");
            saver.SaveNeuralNetwork(bestPerformer.NeuralNet, bestPerformer.GetSessionEvaluation(), epoch);
        }

        internal IList<INeuralNetwork> GetMutatedNetworks(IEnumerable<INeuralNetwork> networksToTryToMutate, double mutateChance)
        {
            //try to mutate session that will live on 1 by 1
            var mutatedTop = new List<INeuralNetwork>();
            bool didMutate;
            foreach (var topPerformer in networksToTryToMutate)
            {
                var net = _mutator.Mutate(topPerformer, mutateChance, out didMutate);
                if (didMutate)
                {
                    mutatedTop.Add(net);
                }
            }
            return mutatedTop;
        }

        internal double DetermineMutateChance()
        {
            var mutateChance = _evolutionConfig.NormalMutationRate;
            if (_history.IsStale())
            {
                mutateChance = _evolutionConfig.HighMutationRate;
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Eval history is stale, setting mutation to HIGH");
            }
            else
            {
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Mutation set to NORMAL");
            }
            return mutateChance;
        }

        private void createNextGeneration(ITrainingSession bestPerformer)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int numberOfTopPerformersToChoose = (int)(_generationConfig.GenerationPopulation * 0.50);
            int numToBreed = (int)(_generationConfig.GenerationPopulation * 0.35);
            int numToGen = (int)(_generationConfig.GenerationPopulation * 0.15);
            if (numberOfTopPerformersToChoose + numToBreed + numToGen < _generationConfig.GenerationPopulation)
            {
                numToGen += _generationConfig.GenerationPopulation -
                            (numberOfTopPerformersToChoose + numToBreed + numToGen);
            }

            var sessions = _generation.GetBestPerformers(numberOfTopPerformersToChoose);
            if (bestPerformer != null)
            {
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Best performer found for creating generation");
                if (sessions.All(s => s.NeuralNet.GetGenes() != bestPerformer.NeuralNet.GetGenes()))
                {
                    LoggerFactory.GetLogger()
                        .Log(LogLevel.Info,
                            $"Best performer adding to sessions with eval {bestPerformer.GetSessionEvaluation()}");
                    sessions[sessions.Count - 1] = bestPerformer;
                    sessions = sessions.OrderByDescending(s => s.GetSessionEvaluation()).ToList();
                    LoggerFactory.GetLogger()
                        .Log(LogLevel.Info, $"session 0 eval: {sessions[0].GetSessionEvaluation()}");
                }
                else
                {
                    LoggerFactory.GetLogger()
                        .Log(LogLevel.Info,
                            $"Best performer already in generation: not adding.");
                }
            }

            _history.AddEval(sessions[0].GetSessionEvaluation());

            var mutateChance = DetermineMutateChance();
            
            IList<INeuralNetwork> children = _breeder.Breed(sessions, numToBreed);
            var newSessions = new List<ITrainingSession>();
            //Allow the very top numToLiveOn sessions to be added to next generation untouched
            int numToLiveOn = sessions.Count / 10;
            var sessionsToLiveOn = sessions.Take(numToLiveOn).ToList();
            newSessions.AddRange(sessionsToLiveOn);

            //try to mutate session that will live on 1 by 1
            var mutatedTop = GetMutatedNetworks(sessionsToLiveOn.Select(s => s.NeuralNet), mutateChance);

            //or each session that lived on that was mutated, remove last top performer, then mutate remaining top performers in batch and add
            sessions = sessions.Skip(numToLiveOn).ToList();
            var sessionSubset = sessions.Take(sessions.Count - mutatedTop.Count);
            IList <INeuralNetwork> toKeepButPossiblyMutate = sessionSubset.Select(session => session.NeuralNet).ToList();
            IList<INeuralNetwork> newNetworks = getNewNetworks(numToGen, mutateChance);

            List<INeuralNetwork> toTryMutate = new List<INeuralNetwork>();
            //try to mutate both new networks as well as all the top performers we wanted to keep
            toTryMutate.AddRange(toKeepButPossiblyMutate);
            toTryMutate.AddRange(newNetworks);
            bool didMutate;
            IList<INeuralNetwork> maybeMutated = _mutator.Mutate(toTryMutate, mutateChance, out didMutate);

            List<INeuralNetwork> allToAdd = new List<INeuralNetwork>();
            allToAdd.AddRange(mutatedTop);
            allToAdd.AddRange(children);
            allToAdd.AddRange(maybeMutated);

            newSessions.AddRange(allToAdd.Select((net, sessionNumber) => new TrainingSession(net, _evaluatableFactory.Create(net), sessionNumber)));
            _generation = new Generation(newSessions, _generationConfig);

            watch.Stop();
            LoggerFactory.GetLogger().Log(LogLevel.Debug, $"create generation runtime (sec): {watch.Elapsed.TotalSeconds}");
            watch.Reset();
        }

        private List<INeuralNetwork> getNewNetworks(int numToGen, double mutateChance)
        {
            List<INeuralNetwork> newNets = new List<INeuralNetwork>();
            for (int i = 0; i < numToGen; i++)
            {
                var hiddenSpecs = determineHiddenLayerSpec(_networkConfig.NumInputNeurons,
                    _networkConfig.NumHiddenLayers, mutateChance);
                INeuralNetwork newNet = _networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, hiddenSpecs);
                newNets.Add(newNet);
            }
            return newNets;
        }

        private IList<int> determineHiddenLayerSpec(int defaultNumNeurons, int defaultNumHiddenLayers, double mutateChance)
        {
            var random = new Random();
            var numHiddenLayers = randomizeNumber(defaultNumHiddenLayers, mutateChance, random);
            var spec = new List<int>();
            for (var i = 0; i < numHiddenLayers; i++)
            {
                var newLayerSize = randomizeNumber(defaultNumNeurons, mutateChance, random);
                spec.Add(newLayerSize);
            }
            return spec;
        }

        private int randomizeNumber(int seedNumber, double randomizeChance, Random random)
        {
            var numToReturn = seedNumber;
            var increase = !(random.NextDouble() <= 0.5);
            while (random.NextDouble() <= randomizeChance)
            {
                if (!increase && numToReturn == 1)
                {
                    break;
                }
                if (!increase)
                {
                    numToReturn--;
                }
                else
                {
                    numToReturn++;
                }
            }
            return numToReturn;
        }

    }
}
