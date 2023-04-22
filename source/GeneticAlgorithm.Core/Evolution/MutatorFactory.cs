﻿using NeuralNetwork.Core.Factories;
using NeuralNetwork.Core.WeightInitializer;

namespace GeneticAlgorithm.Core.Evolution;

public class MutatorFactory : IMutatorFactory
{
    private readonly INeuralNetworkFactory _networkFactory;
    private readonly IWeightInitializer _weightInitializer;

    private MutatorFactory(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer)
    {
        _networkFactory = networkFactory;
        _weightInitializer = weightInitializer;
    }

    public static IMutatorFactory GetInstance(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer)
    {
        return new MutatorFactory(networkFactory, weightInitializer);
    }

    public IMutator Create(MutationConfigurationSettings config)
    {
        return Mutator.GetInstance(_networkFactory, _weightInitializer, config);
    }

    public IMutator Create()
    {
        MutationConfigurationSettings config = new MutationConfigurationSettings
        {
            MutateAxonActivationFunction = true,
            MutateNumberOfHiddenLayers = true,
            MutateNumberOfHiddenNeuronsInLayer = true,
            MutateSomaBiasFunction = true,
            MutateSomaSummationFunction = true,
            MutateSynapseWeights = true
        };
        return Mutator.GetInstance(_networkFactory, _weightInitializer, config);
    }
}