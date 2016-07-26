using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
namespace NeuralNetwork.GeneticAlgorithm.Evolution
{
    public interface IMutator
    {
        INeuralNetwork Mutate(INeuralNetwork network, double mutateChance, out bool didMutate);
        IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance, out bool didMutate);
    }
}
