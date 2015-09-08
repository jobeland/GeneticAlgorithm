using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
namespace NeuralNetwork.GeneticAlgorithm.Evolution
{
    public interface IMutator
    {
        IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance);
    }
}
