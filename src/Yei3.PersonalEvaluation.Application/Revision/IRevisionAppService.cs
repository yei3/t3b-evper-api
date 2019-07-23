using System.Threading.Tasks;
using Yei3.PersonalEvaluation.Revision.Dto;

namespace Yei3.PersonalEvaluation.Revision
{
    public interface IRevisionAppService
    {
        Task ReviseEvaluation(long evaluationId);
        Task FinishEvaluation(long evaluationId);
        Task UnfininshEvaluation(long evaluationId);
        Task UpdateRevisionDate(UpdateRevisionDateInputDto input);
    }
}