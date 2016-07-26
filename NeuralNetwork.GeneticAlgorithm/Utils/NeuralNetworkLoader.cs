using ArtificialNeuralNetwork;
using Newtonsoft.Json;
using System.IO;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.Genes;

namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    public class NeuralNetworkLoader : INeuralNetworkLoader
    {
        private readonly string _directory;

         public NeuralNetworkLoader(string directory)
        {
            _directory = directory;
        }

        public INeuralNetwork LoadNeuralNetwork(string filename)
        {
            var jsonGenes = File.ReadAllText(_directory + filename);
            var networkGene = JsonConvert.DeserializeObject<NeuralNetworkGene>(jsonGenes);
            var network = NeuralNetworkFactory.GetInstance().Create(networkGene);
            return network;
        }
    }
}
