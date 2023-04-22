using NeuralNetworks.Core.Genes;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GeneticAlgorithms.Core.Utils;

public class NeuralNetworkSaver : INeuralNetworkSaver
{
    private readonly string _directory;

    public NeuralNetworkSaver(string directory)
    {
        _directory = directory;
    }

    public string SaveNeuralNetwork(INeuralNetwork network, double networkEvaluation, int epoch)
    {
        NeuralNetworkGene genes = network.GetGenes();

        string jsonString = JsonSerializer.Serialize(genes);

        string minimized = MinifyJson(jsonString);
        string filename = string.Format("\\network_eval_{0}_epoch_{1}_date_{2}.json", networkEvaluation, epoch, DateTime.Now.Ticks);
        File.WriteAllText(_directory + filename, minimized);

        return filename;
    }

    internal string MinifyJson(string json)
    {
        return Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
    }
}