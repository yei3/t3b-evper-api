using System.Threading.Tasks;

namespace Yei3.PersonalEvaluation.Revision
{
    public interface IRevisionAppService
    {
        Task ReviseEvaluation(long evaluationId);
        Task UnreviseEvaluation(long evaluationId);
        Task FinishEvaluation(long evaluationId);
        Task UnfininshEvaluation(long evaluationId);
    }
}