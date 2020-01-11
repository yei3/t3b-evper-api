using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Binnacle.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Binnacle
{
    public class BinnacleAppService : AsyncCrudAppService<ObjectiveBinnacle, ObjectiveBinnacleDto, long, ObjectiveBinnacleGetAllInputDto>, IBinnacleAppService
    {

        private readonly UserManager _userManager;
        private readonly IRepository<NotEvaluableAnswer, long> _notEvaluableAnswerRepository;

        public BinnacleAppService(IRepository<ObjectiveBinnacle, long> repository, UserManager userManager, IRepository<NotEvaluableAnswer, long> notEvaluableAnswerRepository) : base(repository)
        {
            _userManager = userManager;
            _notEvaluableAnswerRepository = notEvaluableAnswerRepository;
        }

        protected override IQueryable<ObjectiveBinnacle> CreateFilteredQuery(ObjectiveBinnacleGetAllInputDto input)
        {
            return base.CreateFilteredQuery(input)
                .Where(binnacleEntry => binnacleEntry.EvaluationQuestionId == input.EvaluationMeasuredQuestionId);
        }

        public override async Task<ObjectiveBinnacleDto> Create(ObjectiveBinnacleDto input)
        {
            ObjectiveBinnacleDto binnacleDto = await base.Create(input);
            await CurrentUnitOfWork.SaveChangesAsync();
            ObjectiveBinnacle binnacle = await Repository
                .GetAll()
                .Include(objectiveBinnacle => objectiveBinnacle.EvaluationQuestion)
                .ThenInclude(question => question.Evaluation)
                .SingleAsync(objectiveBinnacle => objectiveBinnacle.Id == binnacleDto.Id);

            binnacle.EvaluationQuestion.Status = binnacle.EvaluationQuestion.Status == EvaluationQuestionStatus.NoStatus || binnacle.EvaluationQuestion.Status == EvaluationQuestionStatus.Unanswered
                ? EvaluationQuestionStatus.Initiated
                : binnacle.EvaluationQuestion.Status;

            NotEvaluableAnswer pairAnswer = null;
            Evaluations.EvaluationQuestions.NotEvaluableQuestion currentEvaluableQuestion = binnacle.EvaluationQuestion is Evaluations.EvaluationQuestions.NotEvaluableQuestion
                ? binnacle.EvaluationQuestion.As<Evaluations.EvaluationQuestions.NotEvaluableQuestion>()
                : null;

            if (!currentEvaluableQuestion.IsNullOrDeleted())
            {
                NotEvaluableAnswer notEvaluableAnswer = _notEvaluableAnswerRepository
                    .FirstOrDefault(answer => answer.EvaluationQuestionId == currentEvaluableQuestion.Id);

                var x = _notEvaluableAnswerRepository
                    .GetAll()
                    .Include(answer => answer.NotEvaluableQuestion)
                    .ThenInclude(question => question.Evaluation)
                    .Where(answer => answer.Text == currentEvaluableQuestion.NotEvaluableAnswer.Text)
                    .Where(answer => answer.Id != notEvaluableAnswer.Id)
                    .Where(answer =>
                        answer.NotEvaluableQuestion.Evaluation.Term == currentEvaluableQuestion.Evaluation.Term)
                    .OrderByDescending(answer => answer.CreationTime);

                pairAnswer = x
                    .FirstOrDefault(answer =>
                        answer.NotEvaluableQuestion.Evaluation.UserId == currentEvaluableQuestion.Evaluation.UserId);
            }

            if (pairAnswer.IsNullOrDeleted())
            {
                return binnacleDto;
            }

            input.EvaluationQuestionId = pairAnswer.EvaluationQuestionId;
            await base.Create(input);

            return binnacleDto;
        }

        protected override ObjectiveBinnacleDto MapToEntityDto(ObjectiveBinnacle entity)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                ObjectiveBinnacleDto entityDto = base.MapToEntityDto(entity);
                entityDto.UserName = _userManager.Users.Single(user => user.Id == entity.CreatorUserId).FullName;

                return entityDto;
            }
        }
    }
}