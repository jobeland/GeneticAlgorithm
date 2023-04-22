using NeuralNetworks.Core.Factories;
using NeuralNetworks.Core.Genes;
using System.Text.Json;

namespace GeneticAlgorithms.Core.Utils;

public class NeuralNetworkLoader : INeuralNetworkLoader
{
    private readonly string _directory;

    public NeuralNetworkLoader(string directory)
    {
        _directory = directory;
    }

    public INeuralNetwork LoadNeuralNetwork(string filename)
    {
        string jsonGenes = File.ReadAllText(_directory + filename);
        NeuralNetworkGene networkGene = JsonSerializer.Deserialize<NeuralNetworkGene>(jsonGenes) ?? throw new NullReferenceException();
        INeuralNetwork network = NeuralNetworkFactory.GetInstance().Create(networkGene);
        return network;
    }
}