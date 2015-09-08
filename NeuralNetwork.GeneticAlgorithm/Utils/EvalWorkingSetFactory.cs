using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.GeneticAlgorithm.Utils
{
    public class EvalWorkingSetFactory : NeuralNetwork.GeneticAlgorithm.Utils.IEvalWorkingSetFactory
    {
        private EvalWorkingSetFactory() { }

        public static IEvalWorkingSetFactory GetInstance()
        {
            return new EvalWorkingSetFactory();
        }

        public IEvalWorkingSet Create(int size = 50)
        {
            return EvalWorkingSet.GetInstance(size);
        }
    }
}
