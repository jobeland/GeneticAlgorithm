using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
namespace NeuralNetwork.GeneticAlgorithm.Evolution
{
    public interface IBreeder
    {
        IList<INeuralNetwork> Breed(IList<ITrainingSession> sessions, int numToBreed);
    }
}
