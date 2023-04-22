using GeneticAlgorithms.Core.Evaluatable;
using NeuralNetworks.Core.ActivationFunctions;
using NeuralNetworks.Core.Factories;

namespace GeneticAlgorithms.Core.Tests
{
    public class GeneticAlgorithmFactoryTests
    {
        [Fact]
        public void Test1()
        {
            int numInputs = 3;
            int numOutputs = 1;
            int numHiddenLayers = 1;
            int numNeuronsInHiddenLayer = 5;

            INeuralNetworkFactory factory = NeuralNetworkFactory.GetInstance();
            INeuralNetwork network = factory.Create(numInputs, numOutputs, numHiddenLayers, numNeuronsInHiddenLayer);

            network.Should().NotBeNull();
            network.Should().BeAssignableTo<INeuralNetwork>();
            network.Should().BeOfType<NeuralNetwork>();
        }

        [Fact]
        public void Test2()
        {
            Mock<IEvaluatableFactory> mock = new();

            IEvaluatableFactory evaluatableFactory = mock.Object;

            NeuralNetworkConfigurationSettings networkConfig = new()
            {
                NumInputNeurons = 3,
                NumOutputNeurons = 1,
                NumHiddenLayers = 2,
                NumHiddenNeurons = 3,
                SummationFunction = new SimpleSummation(),
                ActivationFunction = new TanhActivationFunction()
            };

            IGeneticAlgorithmFactory factory = GeneticAlgorithmFactory.GetInstance(evaluatableFactory);
            IGeneticAlgorithm evolver = factory.Create(networkConfig);

            evolver.Should().NotBeNull();
            evolver.Should().BeAssignableTo<IGeneticAlgorithm>();
            evolver.Should().BeOfType<GeneticAlgorithm>();
        }
    }
}