namespace GeneticAlgorithms.Core.Evolution;

public class Generation : IGeneration
{
    private readonly GenerationConfigurationSettings _generationConfig;

    private readonly IList<ITrainingSession> _sessions;
    private double[]? _evals;

    public Generation(IList<ITrainingSession> population, GenerationConfigurationSettings generationConfig)
    {
        _sessions = population;
        _generationConfig = generationConfig;
    }

    public ITrainingSession GetBestPerformer()
    {
        int indexToKeep = 0;
        double[] evals = GetEvalsForGeneration();
        for (int performer = 0; performer < evals.Length; performer++)
        {
            double value = evals[performer];
            if (value > evals[indexToKeep])
            {
                indexToKeep = performer;
            }
        }
        return _sessions[indexToKeep];
    }

    public IList<ITrainingSession> GetBestPerformers(int numPerformers)
    {
        if (numPerformers <= 0)
        {
            throw new ArgumentException("x must be greater than zero");
        }
        if (numPerformers > _sessions.Count)
        {
            throw new ArgumentException("x must be less than number of sesssions");
        }

        int[] indicesToKeep = new int[numPerformers];
        for (int i = 0; i < numPerformers; i++)
        {
            indicesToKeep[i] = i;
        }
        double[] evals = GetEvalsForGeneration();
        for (int performer = 0; performer < evals.Length; performer++)
        {
            double value = evals[performer];
            for (int i = 0; i < indicesToKeep.Length; i++)
            {
                if (value > evals[indicesToKeep[i]])
                {
                    int newIndex = performer;
                    // need to shift all of the rest down now
                    for (int indexContinued = i; indexContinued < numPerformers; indexContinued++)
                    {
                        (newIndex, indicesToKeep[indexContinued]) = (indicesToKeep[indexContinued], newIndex);
                    }
                    break;
                }
            }
        }

        List<ITrainingSession> sessionsToReturn = new();
        for (int i = 0; i < numPerformers; i++)
        {
            sessionsToReturn.Add(_sessions[indicesToKeep[i]]);
        }
        return sessionsToReturn;
    }

    public double[] GetEvalsForGeneration()
    {
        if (_evals == null)
        {
            _evals = new double[_sessions.Count];
            for (int i = 0; i < _sessions.Count; i++)
            {
                _evals[i] = _sessions[i].GetSessionEvaluation();
            }
        }
        //TODO: this shouldn't be in Evals anymore, but just called directly off of the training session
        double[] toReturn = new double[_sessions.Count];
        for (int i = 0; i < _sessions.Count; i++)
        {
            toReturn[i] = _evals[i];
        }
        return toReturn;
    }

    public void Run()
    {
        if (_generationConfig.UseMultithreading)
        {
            Parallel.ForEach<ITrainingSession>(_sessions, session =>
            {
                session.Run();
            });
        }
        else
        {
            foreach (ITrainingSession session in _sessions)
            {
                session.Run();
            }
        }
    }
}