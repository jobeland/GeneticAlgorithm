using GeneticAlgorithms.Core.Evaluatable;
using Microsoft.Extensions.Logging;

namespace GeneticAlgorithms.Core;

public class TrainingSession : ITrainingSession
{
    private readonly IEvaluatable _evaluatable;
    private readonly bool _isIdempotent;
    private readonly ILogger _logger;
    private readonly int _sessionNumber;
    private bool _hasStoredSessionEval;
    private double _sessionEval;

    public INeuralNetwork NeuralNet { get; set; }

    public TrainingSession(INeuralNetwork nn, IEvaluatable evaluatable, int sessionNumber, bool isIdempotent = true)
    {
        _logger = ConsoleLogger.GetLogger<TrainingSession>();

        NeuralNet = nn;
        _evaluatable = evaluatable;
        _sessionNumber = sessionNumber;
        _hasStoredSessionEval = false;
        _sessionEval = 0;
        _isIdempotent = isIdempotent;
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

    public void Run()
    {
        _logger.LogDebug($"Starting training session {_sessionNumber}");
        //only run the session if it hasn't already been run before in idempotent sessions
        if (!_hasStoredSessionEval || !_isIdempotent)
        {
            _evaluatable.RunEvaluation();
        }

        _logger.LogDebug($"Stopping training session {_sessionNumber}");
    }
}