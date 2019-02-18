using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;
using Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion
{
    public class NotEvaluableQuestionAppService : AsyncCrudAppService<Evaluations.EvaluationQuestions.NotEvaluableQuestion, NotEvaluableQuestionDto, long, QuestionGetAllInputDto, NotEvaluableQuestionDto, NotEvaluableQuestionUpdateInputDto>, INotEvaluableQuestionAppService
    {

        private readonly UserManager _userManager;

        public NotEvaluableQuestionAppService(IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> repository, UserManager userManager) : base(repository)
        {
            _userManager = userManager;
        }

        public override async Task<NotEvaluableQuestionDto> Create(NotEvaluableQuestionDto input)
        {
            NotEvaluableQuestionDto notEvaluableQuestion = await base.Create(input);
            await CurrentUnitOfWork.SaveChangesAsync();

            Evaluations.EvaluationQuestions.NotEvaluableQuestion currentQuestion = await Repository
                .GetAll()
                .SingleAsync(question => question.Id == notEvaluableQuestion.Id);

            currentQuestion.SetAnswer(notEvaluableQuestion.Id, String.Empty, DateTime.Now);

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(currentQuestion);
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetSummary(long evaluationId)
        {
            return await
                Repository
                    .GetAll()
                    .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                    .Include(evaluationQuestion => evaluationQuestion.Binnacle)
                    .Include(evaluationQuestion => evaluationQuestion.NotEvaluableAnswer)
                    .Include(evaluationQuestion => evaluationQuestion.Section)
                    .ThenInclude(section => section.ParentSection)
                    .Where(evaluationQuestion => evaluationQuestion.Evaluation.Id == evaluationId)
                    .Where(evaluationQuestion => evaluationQuestion.Evaluation.EndDateTime.AddMonths(1) > DateTime.Now)
                    .Where(evaluationQuestion => evaluationQuestion.Section.Name.StartsWith(AppConsts.SectionObjectivesName, StringComparison.CurrentCultureIgnoreCase))
                    .Where(evaluationQuestion => evaluationQuestion.Section.ParentSection.Name.StartsWith(AppConsts.SectionObjectivesName, StringComparison.CurrentCultureIgnoreCase))
                    .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                    {
                        Status = evaluationQuestion.Status,
                        Name = evaluationQuestion.Text,
                        Deliverable = evaluationQuestion.NotEvaluableAnswer.Text,
                        DeliveryDate = evaluationQuestion.NotEvaluableAnswer.CommitmentTime,
                        Id = evaluationQuestion.Id,
                        Binnacle = evaluationQuestion.Binnacle
                            .Select(objectiveBinnacle => new ObjectiveBinnacleValueObject
                            {
                                Id = objectiveBinnacle.Id,
                                EvaluationQuestionId = objectiveBinnacle.EvaluationQuestionId,
                                Text = objectiveBinnacle.Text,
                                CreationTime = objectiveBinnacle.CreationTime,
                                UserName = _userManager.Users.Single(user => user.Id == objectiveBinnacle.CreatorUserId).FullName
                            }).ToList(),
                        isNotEvaluable = true
                    }).ToListAsync();
        }

        public async Task UpdateStatus(UpdateStatusInputDto input)
        {
            Evaluations.EvaluationQuestions.NotEvaluableQuestion question = await GetEntityByIdAsync(input.Id);
            question.Status = input.Status;
            await Repository.UpdateAsync(question);
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