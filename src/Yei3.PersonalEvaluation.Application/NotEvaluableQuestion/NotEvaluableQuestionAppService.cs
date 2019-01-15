using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;
using Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion
{
    public class NotEvaluableQuestionAppService : AsyncCrudAppService<Evaluations.EvaluationQuestions.NotEvaluableQuestion, NotEvaluableQuestionDto, long, QuestionGetAllInputDto>, INotEvaluableQuestionAppService
    {
        public NotEvaluableQuestionAppService(IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> repository) : base(repository)
        {
        }

        public override async Task<NotEvaluableQuestionDto> Create(NotEvaluableQuestionDto input)
        {
            NotEvaluableQuestionDto notEvaluableQuestion = await base.Create(input);
            await CurrentUnitOfWork.SaveChangesAsync();

            Evaluations.EvaluationQuestions.NotEvaluableQuestion currentQuestion = await Repository
                .GetAll()
                .SingleAsync(question => question.Id == notEvaluableQuestion.Id);

            currentQuestion.SetAnswer(notEvaluableQuestion.Id);

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(currentQuestion);
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetSummary(long evaluationId)
        {
            return await Repository
                  .GetAll()
                  .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                  .Where(evaluationQuestion => evaluationQuestion.Evaluation.Id == evaluationId)
                  .Where(evaluationQuestion => evaluationQuestion.Status != EvaluationQuestionStatus.Validated)
                  .Where(evaluationQuestion => evaluationQuestion.Evaluation.EndDateTime > DateTime.Now)
                  .OfType<Evaluations.EvaluationQuestions.NotEvaluableQuestion>()
                  .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                  {
                      Status = evaluationQuestion.Status,
                      Name = evaluationQuestion.Text,
                      Deliverable = evaluationQuestion.NotEvaluableAnswer.Text,
                      DeliveryDate = evaluationQuestion.NotEvaluableAnswer.CommitmentTime,
                      Id = evaluationQuestion.Id,
                      Binnacle = evaluationQuestion.Binnacle.Select(objectiveBinnacle => new ObjectiveBinnacleValueObject
                      {
                          Id = objectiveBinnacle.Id,
                          EvaluationQuestionId = objectiveBinnacle.EvaluationQuestionId,
                          Text = objectiveBinnacle.Text,
                          CreationTime = objectiveBinnacle.CreationTime
                      }).ToList(),
                      isNotEvaluable = true
                  }).ToListAsync();
        }

        protected override IQueryable<Evaluations.EvaluationQuestions.NotEvaluableQuestion> CreateFilteredQuery(QuestionGetAllInputDto input)
        {
            return Repository
                .GetAll()
                .Include(question => question.NotEvaluableAnswer);
        }

        protected override async Task<Evaluations.EvaluationQuestions.NotEvaluableQuestion> GetEntityByIdAsync(long id)
        {
            return await Repository
                .GetAll()
                .Include(question => question.NotEvaluableAnswer)
                .SingleAsync(question => question.Id == id);
        }
    }
}