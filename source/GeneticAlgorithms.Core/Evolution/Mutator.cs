using NeuralNetworks.Core.ActivationFunctions;
using NeuralNetworks.Core.Factories;
using NeuralNetworks.Core.Genes;
using NeuralNetworks.Core.SummationFunctions;
using NeuralNetworks.Core.WeightInitializer;

namespace GeneticAlgorithms.Core.Evolution;

public class Mutator : IMutator
{
    private readonly MutationConfigurationSettings _config;
    private readonly INeuralNetworkFactory _networkFactory;
    private readonly Random _random;
    private readonly IWeightInitializer _weightInitializer;

    private Mutator(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer, MutationConfigurationSettings config)
    {
        _networkFactory = networkFactory;
        _weightInitializer = weightInitializer;
        _config = config;
        _random = new Random();
    }

    public static IMutator GetInstance(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer, MutationConfigurationSettings config)
    {
        return new Mutator(networkFactory, weightInitializer, config);
    }

    public INeuralNetwork Mutate(INeuralNetwork network, double mutateChance, out bool didMutate)
    {
        NeuralNetworkGene childGenes = network.GetGenes();
        bool mutated;
        didMutate = false;
        if (_config.MutateNumberOfHiddenLayers)
        {
            childGenes = TryAddLayerToNetwork(childGenes, mutateChance, out mutated);
            didMutate = mutated;
        }
        for (int n = 0; n < childGenes.InputGene.Neurons.Count; n++)
        {
            NeuronGene neuron = childGenes.InputGene.Neurons[n];
            childGenes.InputGene.Neurons[n] = TryMutateNeuron(neuron, mutateChance, out mutated); ;
            didMutate = didMutate || mutated;
        }

        for (int h = 0; h < childGenes.HiddenGenes.Count; h++)
        {
            if (_config.MutateNumberOfHiddenNeuronsInLayer)
            {
                childGenes.HiddenGenes[h] = TryAddNeuronsToLayer(childGenes, h, mutateChance, out mutated);
                didMutate = didMutate || mutated;
            }

            for (int j = 0; j < childGenes.HiddenGenes[h].Neurons.Count; j++)
            {
                NeuronGene neuron = childGenes.HiddenGenes[h].Neurons[j];
                childGenes.HiddenGenes[h].Neurons[j] = TryMutateNeuron(neuron, mutateChance, out mutated);
                didMutate = didMutate || mutated;
            }
        }
        return _networkFactory.Create(childGenes);
    }

    public IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance, out bool didMutate)
    {
        List<INeuralNetwork> completed = new();
        didMutate = false;
        foreach (INeuralNetwork net in networks)
        {
            completed.Add(Mutate(net, mutateChance, out bool mutated));
            didMutate = didMutate || mutated;
        }
        return completed;
    }

    internal static LayerGene GetNextLayerGene(NeuralNetworkGene genes, int hiddenLayerIndex)
    {
        if (hiddenLayerIndex == genes.HiddenGenes.Count - 1)
        {
            return genes.OutputGene;
        }
        else
        {
            return genes.HiddenGenes[hiddenLayerIndex + 1];
        }
    }

    internal static LayerGene GetPreviousLayerGene(NeuralNetworkGene genes, int hiddenLayerIndex)
    {
        if (hiddenLayerIndex == 0)
        {
            return genes.InputGene;
        }
        else
        {
            return genes.HiddenGenes[hiddenLayerIndex - 1];
        }
    }

    internal int DetermineNumberOfHiddenNeuronsInLayer(NeuralNetworkGene networkGenes, double mutateChance)
    {
        int hiddenLayerSize = networkGenes.InputGene.Neurons.Count;
        bool increase = !(_random.NextDouble() <= 0.5);
        while (_random.NextDouble() <= mutateChance)
        {
            if (!increase && hiddenLayerSize == 1)
            {
                break;
            }
            else if (!increase)
            {
                hiddenLayerSize--;
            }
            else
            {
                hiddenLayerSize++;
            }
        }
        return hiddenLayerSize;
    }

    internal IActivationFunction GetRandomActivationFunction()
    {
        int value = _random.Next(10);
        return value switch
        {
            0 => new TanhActivationFunction(),
            1 => new StepActivationFunction(),
            2 => new SinhActivationFunction(),
            3 => new AbsoluteXActivationFunction(),
            4 => new SechActivationFunction(),
            5 => new InverseActivationFunction(),
            6 => new IdentityActivationFunction(),
            7 => new RectifiedLinearActivationFunction(),
            8 => new LeakyRectifiedLinearActivationFunction(),
            _ => new SigmoidActivationFunction(),
        };
    }

    internal NeuronGene GetRandomHiddenNeuronGene(NeuralNetworkGene networkGenes, int hiddenLayerIndex)
    {
        AxonGene axon = new(GetRandomActivationFunction().GetType(), new List<double>());
        SomaGene soma = new(_weightInitializer.InitializeWeight(), GetRandomSummationFunction().GetType());

        NeuronGene neuronGene = new(soma, axon);

        //update terminals for current neuron
        LayerGene nextlayer = GetNextLayerGene(networkGenes, hiddenLayerIndex);
        for (int i = 0; i < nextlayer.Neurons.Count; i++)
        {
            neuronGene.Axon?.Weights.Add(_weightInitializer.InitializeWeight());
        }
        return neuronGene;
    }

    internal ISummationFunction GetRandomSummationFunction()
    {
        int value = _random.Next(4);
        return value switch
        {
            0 => new MinSummation(),
            1 => new AverageSummation(),
            2 => new MaxSummation(),
            _ => new SimpleSummation(),
        };
    }

    internal NeuralNetworkGene TryAddLayerToNetwork(NeuralNetworkGene genes, double mutateChance, out bool didMutate)
    {
        NeuralNetworkGene newGenes = genes;

        didMutate = false;

        while (_random.NextDouble() <= mutateChance)
        {
            didMutate = true;

            int layerToReplace = _random.Next(newGenes.HiddenGenes.Count);
            int hiddenLayerSize = DetermineNumberOfHiddenNeuronsInLayer(genes, mutateChance);

            //update layer-1 axon terminals
            LayerGene previousLayer = GetPreviousLayerGene(newGenes, layerToReplace);

            foreach (NeuronGene neuron in previousLayer.Neurons)
            {
                neuron.Axon?.Weights.Clear();
                for (int i = 0; i < hiddenLayerSize; i++)
                {
                    neuron.Axon?.Weights.Add(_weightInitializer.InitializeWeight());
                }
            }

            LayerGene newLayer = new(new List<NeuronGene>());

            newGenes.HiddenGenes.Insert(layerToReplace, newLayer);

            for (int i = 0; i < hiddenLayerSize; i++)
            {
                NeuronGene newNeuron = GetRandomHiddenNeuronGene(newGenes, layerToReplace);
                newGenes.HiddenGenes[layerToReplace].Neurons.Add(newNeuron);
            }
        }

        return newGenes;
    }

    internal LayerGene TryAddNeuronsToLayer(NeuralNetworkGene networkGenes, int hiddenLayerIndex, double mutateChance, out bool didMutate)
    {
        LayerGene hiddenLayer = networkGenes.HiddenGenes[hiddenLayerIndex];
        didMutate = false;
        while (_random.NextDouble() <= mutateChance)
        {
            didMutate = true;
            //update layer-1 axon terminals
            LayerGene previousLayer = GetPreviousLayerGene(networkGenes, hiddenLayerIndex);
            foreach (NeuronGene neuron in previousLayer.Neurons)
            {
                neuron.Axon?.Weights.Add(_weightInitializer.InitializeWeight());
            }

            hiddenLayer.Neurons.Add(GetRandomHiddenNeuronGene(networkGenes, hiddenLayerIndex));
        }
        return hiddenLayer;
    }

    internal NeuronGene TryMutateNeuron(NeuronGene gene, double mutateChance, out bool didMutate)
    {
        didMutate = false;

        AxonGene axon = new(gene.Axon?.ActivationFunction, new List<double>());

        SomaGene soma = new(0, gene.Soma?.SummationFunction);

        NeuronGene toReturn = new(soma, axon);

        //weights
        for (int j = 0; j < gene.Axon?.Weights.Count; j++)
        {
            if (_config.MutateSynapseWeights && _random.NextDouble() <= mutateChance)
            {
                didMutate = true;
                double val = _random.NextDouble();
                if (_random.NextDouble() < 0.5)
                {
                    // 50% chance of being negative, being between -1 and 1
                    val = 0 - val;
                }
                toReturn.Axon?.Weights.Add(val);
            }
            else
            {
                toReturn.Axon?.Weights.Add(gene.Axon.Weights[j]);
            }
        }

        //bias
        if (_config.MutateSomaBiasFunction && _random.NextDouble() <= mutateChance)
        {
            didMutate = true;
            double val = _random.NextDouble();
            if (_random.NextDouble() < 0.5)
            {
                // 50% chance of being negative, being between -1 and 1
                val = 0 - val;
            }
            toReturn.Soma!.Bias = val;
        }
        else
        {
            toReturn.Soma!.Bias = gene.Soma?.Bias ?? 0d;
        }

        //activation
        if (_config.MutateAxonActivationFunction && _random.NextDouble() <= mutateChance)
        {
            didMutate = true;
            toReturn.Axon!.ActivationFunction = GetRandomActivationFunction().GetType();
        }
        else
        {
            toReturn.Axon!.ActivationFunction = gene.Axon?.ActivationFunction;
        }

        //summation
        if (_config.MutateSomaSummationFunction && _random.NextDouble() <= mutateChance)
        {
            didMutate = true;
            toReturn.Soma.SummationFunction = GetRandomSummationFunction().GetType();
        }
        else
        {
            toReturn.Soma.SummationFunction = gene.Soma?.SummationFunction;
        }
        return gene;
    }
}