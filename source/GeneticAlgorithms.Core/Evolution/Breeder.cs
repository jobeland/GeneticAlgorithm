using NeuralNetworks.Core.Factories;
using NeuralNetworks.Core.Genes;
using NeuralNetworks.Core.WeightInitializer;

namespace GeneticAlgorithms.Core.Evolution;

public class Breeder : IBreeder
{
    private readonly double _motherFatherBias;
    private readonly INeuralNetworkFactory _networkFactory;
    private readonly IWeightInitializer _weightInitializer;

    private Breeder(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer, double motherFatherBias = 0.5)
    {
        _networkFactory = networkFactory;
        _weightInitializer = weightInitializer;
        _motherFatherBias = motherFatherBias;
    }

    public static IBreeder GetInstance(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer, double motherFatherBias = 0.5)
    {
        return new Breeder(networkFactory, weightInitializer, motherFatherBias);
    }

    public IList<INeuralNetwork> Breed(IList<ITrainingSession> sessions, int numToBreed)
    {
        WeightedSessionList weightedSessions = new(sessions);
        List<INeuralNetwork> children = new();
        for (int bred = 0; bred < numToBreed; bred++)
        {
            // choose mother
            ITrainingSession session1 = weightedSessions.ChooseRandomWeightedSession();
            INeuralNetwork mother = session1.NeuralNet;

            // choose father
            ITrainingSession session2 = weightedSessions.ChooseRandomWeightedSession();
            INeuralNetwork father = session2.NeuralNet;

            INeuralNetwork child = Mate(mother, father);
            children.Add(child);
        }

        return children;
    }

    internal NeuronGene AdjustAxonTerminalsOfNeuronGene(NeuronGene gene, int desiredNumberOfTerminals)
    {
        NeuronGene toReturn = new()
        {
            Axon = new AxonGene(),
            Soma = new SomaGene()
        };

        toReturn.Axon.ActivationFunction = gene.Axon?.ActivationFunction;
        toReturn.Axon.Weights = new List<double>();
        toReturn.Soma.SummationFunction = gene.Soma?.SummationFunction;
        toReturn.Soma.Bias = gene.Soma?.Bias ?? 0d;

        if (desiredNumberOfTerminals > gene.Axon?.Weights.Count)
        {
            for (int i = 0; i < gene.Axon.Weights.Count; i++)
            {
                toReturn.Axon.Weights.Add(gene.Axon.Weights[i]);
            }
            int delta = desiredNumberOfTerminals - gene.Axon.Weights.Count;
            for (int j = 0; j < delta; j++)
            {
                toReturn.Axon.Weights.Add(_weightInitializer.InitializeWeight());
            }
        }
        else
        {
            for (int i = 0; i < desiredNumberOfTerminals; i++)
            {
                toReturn.Axon.Weights.Add(gene.Axon?.Weights[i] ?? throw new NullReferenceException());
            }
        }

        return toReturn;
    }

    internal NeuronGene BreedNeuron(NeuronGene father, NeuronGene mother, Random random)
    {
        NeuronGene toReturn = new()
        {
            Axon = new AxonGene(),
            Soma = new SomaGene()
        };

        if (father.Axon?.Weights.Count >= mother.Axon?.Weights.Count)
        {
            toReturn.Axon.Weights = MateAxonWeights(father, mother, random);
        }
        else
        {
            toReturn.Axon.Weights = MateAxonWeights(mother, father, random);
        }

        if (random.NextDouble() < _motherFatherBias)
        {
            toReturn.Axon.ActivationFunction = mother.Axon?.ActivationFunction;
        }
        else
        {
            toReturn.Axon.ActivationFunction = father.Axon?.ActivationFunction;
        }

        if (random.NextDouble() < _motherFatherBias)
        {
            toReturn.Soma.SummationFunction = mother.Soma?.SummationFunction;
        }
        else
        {
            toReturn.Soma.SummationFunction = father.Soma?.SummationFunction;
        }

        if (random.NextDouble() < _motherFatherBias)
        {
            toReturn.Soma.Bias = mother.Soma?.Bias ?? 0d;
        }
        else
        {
            toReturn.Soma.Bias = father.Soma?.Bias ?? 0d;
        }

        return toReturn;
    }

    internal INeuralNetwork Mate(INeuralNetwork mother, INeuralNetwork father)
    {
        NeuralNetworkGene motherGenes = mother.GetGenes();
        NeuralNetworkGene childFatherGenes = father.GetGenes();
        Random random = new();

        for (int n = 0; n < childFatherGenes.InputGene.Neurons.Count; n++)
        {
            NeuronGene neuron = childFatherGenes.InputGene.Neurons[n];
            NeuronGene motherNeuron = motherGenes.InputGene.Neurons[n];

            childFatherGenes.InputGene.Neurons[n] = BreedNeuron(neuron, motherNeuron, random);
        }

        if (childFatherGenes.HiddenGenes.Count >= motherGenes.HiddenGenes.Count)
        {
            childFatherGenes.HiddenGenes = MateHiddenLayers(childFatherGenes.HiddenGenes, motherGenes.HiddenGenes, random);
        }
        else
        {
            childFatherGenes.HiddenGenes = MateHiddenLayers(motherGenes.HiddenGenes, childFatherGenes.HiddenGenes, random);
        }

        for (int n = 0; n < childFatherGenes.OutputGene.Neurons.Count; n++)
        {
            NeuronGene neuron = childFatherGenes.OutputGene.Neurons[n];
            NeuronGene motherNeuron = motherGenes.OutputGene.Neurons[n];

            childFatherGenes.OutputGene.Neurons[n] = BreedNeuron(neuron, motherNeuron, random);
        }

        INeuralNetwork child = _networkFactory.Create(childFatherGenes);
        return child;
    }

    internal IList<double> MateAxonWeights(NeuronGene moreTerminals, NeuronGene lessTerminals, Random random)
    {
        List<double> weights = new();
        for (int j = 0; j < moreTerminals.Axon?.Weights.Count; j++)
        {
            if (random.NextDouble() < _motherFatherBias && j < lessTerminals.Axon?.Weights.Count)
            {
                weights.Add(lessTerminals.Axon.Weights[j]);
            }
            else
            {
                weights.Add(moreTerminals.Axon.Weights[j]);
            }
        }
        return weights;
    }

    internal IList<LayerGene> MateHiddenLayers(IList<LayerGene> moreLayers, IList<LayerGene> lessLayers, Random random)
    {
        List<LayerGene> matedLayers = new();
        for (int h = 0; h < moreLayers.Count; h++)
        {
            //check to make sure they both have that hidden layer and only breed that layer if they both do. otherwise just keep it untouched
            if (h < lessLayers.Count)
            {
                if (moreLayers[h].Neurons.Count >= lessLayers[h].Neurons.Count)
                {
                    matedLayers.Add(MateLayer(moreLayers[h], lessLayers[h], random));
                }
                else
                {
                    matedLayers.Add(MateLayer(lessLayers[h], moreLayers[h], random));
                }
            }
            else
            {
                matedLayers.Add(moreLayers[h]);
            }
        }
        return matedLayers;
    }

    internal LayerGene MateLayer(LayerGene moreNeurons, LayerGene lessNeurons, Random random)
    {
        LayerGene childGene = new(new List<NeuronGene>());

        bool sameNumberOfTerminalsPerNeuronForBothMates = moreNeurons.Neurons[0].Axon?.Weights.Count == lessNeurons.Neurons[0].Axon?.Weights.Count;
        int maxTerminals = Math.Max(moreNeurons.Neurons[0].Axon?.Weights.Count ?? throw new NullReferenceException(), lessNeurons.Neurons[0].Axon?.Weights.Count ?? throw new NullReferenceException());

        for (int j = 0; j < moreNeurons.Neurons.Count; j++)
        {
            //only breed the neuron if both mates have it. Otherwise just leave add the extra neuron untouched.
            if (j < lessNeurons.Neurons.Count)
            {
                NeuronGene neuron = moreNeurons.Neurons[j];
                NeuronGene lessNeuron = lessNeurons.Neurons[j];
                childGene.Neurons.Add(BreedNeuron(neuron, lessNeuron, random));
            }
            else
            {
                if (sameNumberOfTerminalsPerNeuronForBothMates)
                {
                    childGene.Neurons.Add(moreNeurons.Neurons[j]);
                }
                else
                {
                    childGene.Neurons.Add(AdjustAxonTerminalsOfNeuronGene(moreNeurons.Neurons[j], maxTerminals));
                }
            }
        }
        return childGene;
    }
}