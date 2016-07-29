using ArtificialNeuralNetwork;
using Logging;
using NeuralNetwork.GeneticAlgorithm.Evaluatable;

namespace NeuralNetwork.GeneticAlgorithm
{

    public class TrainingSession : ITrainingSession
    {
        private readonly int _sessionNumber;
        public INeuralNetwork NeuralNet { get; set; }
        private readonly IEvaluatable _evaluatable;
        private bool _hasStoredSessionEval;
        private double _sessionEval;
        private readonly bool _isIdempotent;

        public TrainingSession(INeuralNetwork nn, IEvaluatable evaluatable, int sessionNumber, bool isIdempotent = true)
        {
            NeuralNet = nn;
            _evaluatable = evaluatable;
            _sessionNumber = sessionNumber;
            _hasStoredSessionEval = false;
            _sessionEval = 0;
            _isIdempotent = isIdempotent;
        }

        public void Run()
        {
            LoggerFactory.GetLogger().Log(LogLevel.Debug, $"Starting training session {_sessionNumber}");
            //only run the session if it hasn't already been run before in idempotent sessions
            if (!_hasStoredSessionEval || !_isIdempotent)
            {
                _evaluatable.RunEvaluation();
            }

            LoggerFactory.GetLogger().Log(LogLevel.Debug, $"Stopping training session {_sessionNumber}");
        }

        public double GetSessionEvaluation()
        {

            if (_hasStoredSessionEval && _isIdempotent)
            {
                return _sessionEval;
            }
            _sessionEval = _evaluatable.GetEvaluation();
            _hasStoredSessionEval = true;
            return _sessionEval;
        }
    }
}
