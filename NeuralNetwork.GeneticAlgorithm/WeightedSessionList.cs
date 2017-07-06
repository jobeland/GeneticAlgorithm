using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using NeuralNetwork.GeneticAlgorithm.Utils;

namespace NeuralNetwork.GeneticAlgorithm
{
    public class WeightedSessionList
    {
        private IList<WeightedSession> _sessions;

        public WeightedSessionList(IList<ITrainingSession> sessions)
        {
            double sumOfAllEvals = 0;
            for (int i = 0; i < sessions.Count; i++)
            {
                sumOfAllEvals += sessions[i].GetSessionEvaluation();
            }
            if (sumOfAllEvals <= 0)
            {
                sumOfAllEvals = 1;
            }

            List<WeightedSession> toChooseFrom = new List<WeightedSession>();
            double cumulative = 0.0;
            for (int i = 0; i < sessions.Count; i++)
            {
                //TODO: this weight determination algorithm should be delegated
                double value = sessions[i].GetSessionEvaluation();
                double weight = value / sumOfAllEvals;
                WeightedSession weightedSession = new WeightedSession
                {
                    Session = sessions[i],
                    Weight = weight,
                };
                toChooseFrom.Add(weightedSession);
            }

            toChooseFrom = toChooseFrom.OrderBy(session => session.Weight).ToList();
            foreach (WeightedSession session in toChooseFrom)
            {
                cumulative += session.Weight;
                session.CumlativeWeight = cumulative;
            }
            _sessions = toChooseFrom;
        }

        public ITrainingSession ChooseRandomWeightedSession()
        {
            var value = RandomGenerator.GetInstance().NextDouble() * _sessions[_sessions.Count - 1].CumlativeWeight;
            var weightedSession = _sessions.LastOrDefault(s => s.CumlativeWeight <= value);
            return weightedSession == null ? _sessions[0].Session : weightedSession.Session;
        }
    }
}
