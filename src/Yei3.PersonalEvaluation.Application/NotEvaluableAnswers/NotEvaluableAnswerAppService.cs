using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.NotEvaluableAnswers.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableAnswers
{
    public class NotEvaluableAnswerAppService : AsyncCrudAppService<NotEvaluableAnswer, NotEvaluableAnswerDto, long, GetAllEvaluationObjectivesDto>, INotEvaluableAnswerAppService
    {
        public NotEvaluableAnswerAppService(IRepository<NotEvaluableAnswer, long> repository) : base(repository)
        {
        }

        public override async Task<NotEvaluableAnswerDto> Update(NotEvaluableAnswerDto input)
        {
            NotEvaluableAnswer currentEvaluableAnswer = Repository
                .GetAll()
                .Where(answer => answer.Id == input.Id)
                .Include(answer => answer.NotEvaluableQuestion)
                .ThenInclude(question => question.Evaluation)
                .FirstOrDefault();

            NotEvaluableAnswer pairAnswer = null;

            if (!currentEvaluableAnswer.NotEvaluableQuestion.IsNullOrDeleted())
            {
                if (!currentEvaluableAnswer.NotEvaluableQuestion.Evaluation.IsNullOrDeleted())
                {
                    var x = Repository
                        .GetAll()
                        .Include(answer => answer.NotEvaluableQuestion)
                        .ThenInclude(question => question.Evaluation)
                        .Where(answer => answer.NotEvaluableQuestion.Text == currentEvaluableAnswer.NotEvaluableQuestion.Text)
                        .Where(answer => answer.Id != currentEvaluableAnswer.Id)
                        .Where(answer =>
                            answer.NotEvaluableQuestion.Evaluation.Term ==
                            currentEvaluableAnswer.NotEvaluableQuestion.Evaluation.Term)
                        .OrderByDescending(answer => answer.CreationTime);

                    pairAnswer = x
                        .FirstOrDefault(answer => answer.NotEvaluableQuestion.Evaluation.UserId ==
                                                   currentEvaluableAnswer.NotEvaluableQuestion.Evaluation.UserId);
                }
            }
            
            NotEvaluableAnswerDto result = await base.Update(input);

            if (pairAnswer.IsNullOrDeleted())
            {
                return result;
            }

            input.Id = pairAnswer.Id;
            input.EvaluationQuestionId = pairAnswer.EvaluationQuestionId;

            await base.Update(input);

            return result;
        }
    }
}