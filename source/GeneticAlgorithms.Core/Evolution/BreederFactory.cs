using NeuralNetworks.Core.Factories;
using NeuralNetworks.Core.WeightInitializer;

namespace GeneticAlgorithms.Core.Evolution;

public class BreederFactory : IBreederFactory
{
    private readonly INeuralNetworkFactory _networkFactory;
    private readonly IWeightInitializer _weightInitializer;

    private BreederFactory(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer)
    {
        _networkFactory = networkFactory;
        _weightInitializer = weightInitializer;
    }

    public static IBreederFactory GetInstance(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer)
    {
        return new BreederFactory(networkFactory, weightInitializer);
    }

    public IBreeder Create(double motherFatherBias = 0.5)
    {
        return Breeder.GetInstance(_networkFactory, _weightInitializer, motherFatherBias);
    }
}