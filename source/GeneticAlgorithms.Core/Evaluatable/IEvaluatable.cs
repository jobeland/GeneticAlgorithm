namespace GeneticAlgorithms.Core.Evaluatable;

public interface IEvaluatable
{
    double GetEvaluation();

    void RunEvaluation();
}