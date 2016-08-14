using ArtificialNeuralNetwork;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    public class NeuralNetworkSaver : INeuralNetworkSaver
    {
        private readonly string _directory;

        public NeuralNetworkSaver(string directory)
        {
            _directory = directory;
        }

        public string SaveNeuralNetwork(INeuralNetwork network, double networkEvaluation, int epoch)
        {
            var genes = network.GetGenes();
            var json = JsonConvert.SerializeObject(genes);
            var minimized = MinifyJson(json);
            var filename = string.Format("network_eval_{0}_epoch_{1}_date_{2}.json", networkEvaluation, epoch, DateTime.Now.Ticks);
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);
            File.WriteAllText(Path.Combine(_directory, filename), minimized);
            return filename;
        }

        internal string MinifyJson(string json)
        {
            return Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
        }
    }
}
