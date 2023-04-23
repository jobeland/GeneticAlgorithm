using GeneticAlgorithms.Core.Evaluatable;
using GeneticAlgorithms.Core.Evolution;
using GeneticAlgorithms.Core.Utils;
using Microsoft.Extensions.Logging;
using NeuralNetworks.Core.Factories;
using System.Diagnostics;
using System.Text;

namespace GeneticAlgorithms.Core;

public class GeneticAlgorithm : IGeneticAlgorithm
{
    private readonly IBreeder _breeder;
    private readonly IEpochAction? _epochAction;
    private readonly IEvaluatableFactory _evaluatableFactory;
    private readonly EvolutionConfigurationSettings _evolutionConfig;
    private readonly GenerationConfigurationSettings _generationConfig;
    private readonly IEvalWorkingSet _history;
    private readonly ILogger _logger;
    private readonly IMutator _mutator;
    private readonly NeuralNetworkConfigurationSettings _networkConfig;
    private readonly INeuralNetworkFactory _networkFactory;

    private ITrainingSession? _bestPerformerOfEpoch;
    private IGeneration _generation;

    private GeneticAlgorithm(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory,
            IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction? epochAction)
    {
        _logger = ConsoleLogger.GetLogger<GeneticAlgorithm>();
        _networkConfig = networkConfig;
        _generationConfig = generationConfig;
        _evolutionConfig = evolutionConfig;

        _epochAction = epochAction;

        IList<ITrainingSession> sessions = new List<ITrainingSession>();

        _networkFactory = networkFactory;
        _breeder = breeder;
        _mutator = mutator;
        _history = workingSet;
        _evaluatableFactory = evaluatableFactory;

        for (int i = 0; i < _generationConfig.GenerationPopulation; i++)
        {
            INeuralNetwork network = _networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, _networkConfig.NumHiddenLayers, _networkConfig.NumHiddenNeurons);
            sessions.Add(new TrainingSession(network, _evaluatableFactory.Create(network), i));
        }
        _generation = new Generation(sessions, _generationConfig);
    }

    public static IGeneticAlgorithm GetInstance(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig,
        INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory, IEpochAction? epochAction)
    {
        return new GeneticAlgorithm(networkConfig, generationConfig, evolutionConfig, networkFactory, breeder, mutator, workingSet, evaluatableFactory, epochAction);
    }

    public INeuralNetwork GetBestPerformer()
    {
        if (_bestPerformerOfEpoch == null)
        {
            throw new InvalidOperationException("Cannot return best performer before simulation is complete");
        }
        return _bestPerformerOfEpoch.NeuralNet;
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
                    _logger.LogInformation("Creating next generation with top performer with eval: {BestPerformerEvaluation}", _bestPerformerOfEpoch?.GetSessionEvaluation());

                    CreateNextGeneration(_bestPerformerOfEpoch);
                }
                else
                {
                    CreateNextGeneration(null);
                }
                _generation.Run();

                IEnumerable<double> evals = _generation.GetEvalsForGeneration().OrderByDescending(d => d);
                int evalsTotalCount = evals.Count();

                if (_evolutionConfig.NumTopEvalsToReport > 0)
                {
                    evals = evals.Take(_evolutionConfig.NumTopEvalsToReport);
                }

                StringBuilder sb = new();
                foreach (double t in evals.OrderBy(d => d))
                {
                    sb.AppendLine($"eval: {t}");
                }

                _logger.LogInformation("{Evaluations}", sb.ToString());
                _logger.LogInformation("count: {EvaluationsTotalCount}", evalsTotalCount);
                _logger.LogInformation("Epoch: {Epoch},  Generation: {Generation}", epoch, generation);
            }
            if (_epochAction != null)
            {
                _logger.LogInformation("Updating best performer");
                _bestPerformerOfEpoch = _epochAction.UpdateBestPerformer(_generation, epoch);
            }
            else
            {
                _bestPerformerOfEpoch = GetBestPerformerOfGeneration();
            }
            SaveBestPerformer(epoch);
        }
    }

    internal double DetermineMutateChance()
    {
        double mutateChance = _evolutionConfig.NormalMutationRate;
        if (_history.IsStale())
        {
            mutateChance = _evolutionConfig.HighMutationRate;
            _logger.LogInformation("Eval history is stale, setting mutation to HIGH");
        }
        else
        {
            _logger.LogInformation("Mutation set to NORMAL");
        }
        return mutateChance;
    }

    internal ITrainingSession GetBestPerformerOfGeneration()
    {
        if (_generation == null)
        {
            throw new InvalidOperationException("Cannot return best performer before generation is simulated");
        }
        return _generation.GetBestPerformer();
    }

    internal IList<INeuralNetwork> GetMutatedNetworks(IEnumerable<INeuralNetwork> networksToTryToMutate, double mutateChance)
    {
        //try to mutate session that will live on 1 by 1
        List<INeuralNetwork> mutatedTop = new();
        foreach (INeuralNetwork topPerformer in networksToTryToMutate)
        {
            INeuralNetwork net = _mutator.Mutate(topPerformer, mutateChance, out bool didMutate);
            if (didMutate)
            {
                mutatedTop.Add(net);
            }
        }
        return mutatedTop;
    }

    internal void RunStarterGeneration()
    {
        _generation.Run();
        if (_epochAction != null)
        {
            _logger.LogInformation("Getting best performer");
            _bestPerformerOfEpoch = _epochAction.UpdateBestPerformer(_generation, 0);
        }
        else
        {
            _bestPerformerOfEpoch = GetBestPerformerOfGeneration();
        }
    }

    internal void SaveBestPerformer(int epoch)
    {
        ITrainingSession bestPerformer = _generation.GetBestPerformer();
        NeuralNetworkSaver saver = new("\\networks");
        saver.SaveNeuralNetwork(bestPerformer.NeuralNet, bestPerformer.GetSessionEvaluation(), epoch);
    }

    private static IList<int> DetermineHiddenLayerSpec(int defaultNumNeurons, int defaultNumHiddenLayers, double mutateChance)
    {
        Random random = new();
        int numHiddenLayers = RandomizeNumber(defaultNumHiddenLayers, mutateChance, random);
        List<int> spec = new();
        for (int i = 0; i < numHiddenLayers; i++)
        {
            int newLayerSize = RandomizeNumber(defaultNumNeurons, mutateChance, random);
            spec.Add(newLayerSize);
        }
        return spec;
    }

    private static int RandomizeNumber(int seedNumber, double randomizeChance, Random random)
    {
        int numToReturn = seedNumber;
        bool increase = !(random.NextDouble() <= 0.5);
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

    private void CreateNextGeneration(ITrainingSession? bestPerformer)
    {
        Stopwatch watch = new();
        watch.Start();

        int numberOfTopPerformersToChoose = (int)(_generationConfig.GenerationPopulation * 0.50);
        int numToBreed = (int)(_generationConfig.GenerationPopulation * 0.35);
        int numToGen = (int)(_generationConfig.GenerationPopulation * 0.15);
        if (numberOfTopPerformersToChoose + numToBreed + numToGen < _generationConfig.GenerationPopulation)
        {
            numToGen += _generationConfig.GenerationPopulation -
                        (numberOfTopPerformersToChoose + numToBreed + numToGen);
        }

        IList<ITrainingSession> sessions = _generation.GetBestPerformers(numberOfTopPerformersToChoose);
        if (bestPerformer != null)
        {
            _logger.LogInformation("Best performer found for creating generation");

            if (sessions.All(s => s.NeuralNet.GetGenes() != bestPerformer.NeuralNet.GetGenes()))
            {
                _logger.LogInformation("Best performer adding to sessions with eval {BestPerformerEvaluation}", bestPerformer.GetSessionEvaluation());

                sessions[sessions.Count - 1] = bestPerformer;
                sessions = sessions.OrderByDescending(s => s.GetSessionEvaluation()).ToList();

                _logger.LogInformation("session 0 eval: {SessionEvaluation}", sessions[0].GetSessionEvaluation());
            }
            else
            {
                _logger.LogInformation($"Best performer already in generation: not adding.");
            }
        }

        _history.AddEval(sessions[0].GetSessionEvaluation());

        double mutateChance = DetermineMutateChance();

        IList<INeuralNetwork> children = _breeder.Breed(sessions, numToBreed);
        List<ITrainingSession> newSessions = new();
        //Allow the very top numToLiveOn sessions to be added to next generation untouched
        int numToLiveOn = sessions.Count / 10;
        List<ITrainingSession> sessionsToLiveOn = sessions.Take(numToLiveOn).ToList();
        newSessions.AddRange(sessionsToLiveOn);

        //try to mutate session that will live on 1 by 1
        IList<INeuralNetwork> mutatedTop = GetMutatedNetworks(sessionsToLiveOn.Select(s => s.NeuralNet), mutateChance);

        //or each session that lived on that was mutated, remove last top performer, then mutate remaining top performers in batch and add
        sessions = sessions.Skip(numToLiveOn).ToList();
        IEnumerable<ITrainingSession> sessionSubset = sessions.Take(sessions.Count - mutatedTop.Count);
        IList<INeuralNetwork> toKeepButPossiblyMutate = sessionSubset.Select(session => session.NeuralNet).ToList();
        IList<INeuralNetwork> newNetworks = GetNewNetworks(numToGen, mutateChance);

        List<INeuralNetwork> toTryMutate = new();
        //try to mutate both new networks as well as all the top performers we wanted to keep
        toTryMutate.AddRange(toKeepButPossiblyMutate);
        toTryMutate.AddRange(newNetworks);
        IList<INeuralNetwork> maybeMutated = _mutator.Mutate(toTryMutate, mutateChance, out bool didMutate);

        List<INeuralNetwork> allToAdd = new();
        allToAdd.AddRange(mutatedTop);
        allToAdd.AddRange(children);
        allToAdd.AddRange(maybeMutated);

        newSessions.AddRange(allToAdd.Select((net, sessionNumber) => new TrainingSession(net, _evaluatableFactory.Create(net), sessionNumber)));
        _generation = new Generation(newSessions, _generationConfig);

        watch.Stop();
        _logger.LogInformation("create generation runtime (sec): {TotalSeconds}", watch.Elapsed.TotalSeconds);
        watch.Reset();
    }

    private List<INeuralNetwork> GetNewNetworks(int numToGen, double mutateChance)
    {
        List<INeuralNetwork> newNets = new();
        for (int i = 0; i < numToGen; i++)
        {
            IList<int> hiddenSpecs = DetermineHiddenLayerSpec(_networkConfig.NumInputNeurons,
                _networkConfig.NumHiddenLayers, mutateChance);
            INeuralNetwork newNet = _networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, hiddenSpecs);
            newNets.Add(newNet);
        }
        return newNets;
    }
}