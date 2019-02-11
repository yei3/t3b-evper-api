using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
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

        public BinnacleAppService(IRepository<ObjectiveBinnacle, long> repository, UserManager userManager) : base(repository)
        {
            _userManager = userManager;
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
            NotEvaluableAnswer currentEvaluableAnswer = binnacle.EvaluationQuestion.Evaluation is NotEvaluableAnswer
                ? binnacle.EvaluationQuestion.Evaluation.As<NotEvaluableAnswer>()
                : null;

            if (!currentEvaluableAnswer.IsNullOrDeleted())
            {
                pairAnswer = _notEvaluableAnswerRepository
                    .GetAll()
                    .Include(answer => answer.NotEvaluableQuestion)
                    .ThenInclude(question => question.Evaluation)
                    .Where(answer => answer.Text == currentEvaluableAnswer.Text)
                    .Where(answer => answer.Id != currentEvaluableAnswer.Id)
                    .Where(answer => answer.NotEvaluableQuestion.Evaluation.Term == currentEvaluableAnswer.NotEvaluableQuestion.Evaluation.Term)
                    .SingleOrDefault(answer => answer.NotEvaluableQuestion.Evaluation.UserId ==
                                               currentEvaluableAnswer.NotEvaluableQuestion.Evaluation.UserId);

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
            ObjectiveBinnacleDto entityDto = base.MapToEntityDto(entity);
            entityDto.UserName = _userManager.Users.Single(user => user.Id == entity.CreatorUserId).FullName;

            return entityDto;
        }
    }
}