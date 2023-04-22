namespace GeneticAlgorithm.Core.Evaluatable;

public interface IEvaluatable
{
    double GetEvaluation();

    void RunEvaluation();
}