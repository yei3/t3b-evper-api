using Abp.Application.Services;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.Objective
{
    public class ObjectiveAppService : AsyncCrudAppService<MeasuredQuestion, QuestionDto, long, QuestionGetAllInputDto, QuestionDto>, IObjectiveAppService
    {
        public ObjectiveAppService(IRepository<MeasuredQuestion, long> repository) : base(repository)
        {
        }
    }
}