namespace Yei3.PersonalEvaluation.Evaluations
{
    using System.Collections.Generic;
    using Abp.Application.Services;
    using Abp.Domain.Repositories;
    using Dto;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Abp.Linq.Extensions;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Abp.UI;
    using ValueObjects;
    using Abp.AutoMapper;

    public class EvaluationAppService : AsyncCrudAppService<Evaluation, EvaluationDto, long, GetAllEvaluationsInputDto, CreateEvaluationDto>, IEvaluationAppService
    {
        private readonly IEvaluationManager EvaluationManager;

        public EvaluationAppService(IRepository<Evaluation, long> repository, IEvaluationManager evaluationManager) : base(repository)
        {
            EvaluationManager = evaluationManager;
        }

        protected override IQueryable<Evaluation> CreateFilteredQuery(GetAllEvaluationsInputDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(evaluation => evaluation.EvaluatorUser)
                .Include(evaluation => evaluation.Sections)
                .WhereIf(input.CreatorUserId.HasValue, evaluation => evaluation.CreatorUserId == input.CreatorUserId)
                .WhereIf(input.EvaluatorUserId.HasValue, evaluation => evaluation.EvaluatorUserId == input.EvaluatorUserId)
                .WhereIf(input.MinTime.HasValue, evaluation => evaluation.CreationTime >= input.MinTime.Value)
                .WhereIf(input.MaxTime.HasValue, evaluation => evaluation.CreationTime >= input.MaxTime.Value);
        }

        protected override async Task<Evaluation> GetEntityByIdAsync(long id)
        {
            return await Repository
                .GetAllIncluding(evaluation => evaluation.EvaluatorUser)
                .Include(evaluation => evaluation.Sections)
                .FirstAsync(evaluation => evaluation.Id == id);
        }

        public async Task<EntityDto<long>> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveDto addEvaluationObjectiveDto)
        {
            try
            {
                return new EntityDto<long>(await EvaluationManager.AddEvaluationObjectiveAndGetIdAsync(
                    new AddEvaluationObjectiveValueObject
                    {
                        EvaluationId = addEvaluationObjectiveDto.EvaluationId,
                        Index = addEvaluationObjectiveDto.Index,
                        Description = addEvaluationObjectiveDto.Description,
                        DefinitionOfDone = addEvaluationObjectiveDto.DefinitionOfDone,
                        DeliveryDate = addEvaluationObjectiveDto.DeliveryDate
                    }));
            }
            catch (DbUpdateException e)
            {
                throw new UserFriendlyException(L(e.Message));
            }
        }

        public async Task<EntityDto<long>> AddEvaluationSectionAndGetIdAsync(SectionDto sectionDto)
        {
            try
            {
                var x = await EvaluationManager.AddEvaluationSectionAndGetIdAsync(
                    sectionDto.MapTo<SectionValueObject>());
                return new EntityDto<long>(x);
            }
            catch (DbUpdateException e)
            {
                throw new UserFriendlyException(L(e.Message));
            }
        }

        public async Task<EntityDto<long>> AddEvaluationInstructionsAndGetIdAsync(SetEvaluationInstructionsDto evaluationInstructionsDto)
        {
            try
            {

                return new EntityDto<long>(await EvaluationManager.AddEvaluationInstructionsAndGetIdAsync(new AddEvaluationInstructionsValueObject
                {
                    Id = evaluationInstructionsDto.Id,
                    Instructions = evaluationInstructionsDto.Instructions
                }));
            }
            catch (DbUpdateException e)
            {
                throw new UserFriendlyException(L(e.Message));
            }
        }

        public async Task<ICollection<EntityDto<long>>> EvaluateUsersAndGetIdsAsync(EvaluateUsersInputDto evaluateUsersInputDto)
        {
            try
            {
                List<long> userEvaluationIds = new List<long>(
                    await EvaluationManager.EvaluateUsers(evaluateUsersInputDto.EvaluationId,
                        evaluateUsersInputDto.EvaluatedUserIds));

                return userEvaluationIds.Select(userEvaluationId => new EntityDto<long>(userEvaluationId)).ToList();
            }
            catch (DbUpdateException e)
            {
                throw new UserFriendlyException(L(e.Message));
            }
        }
    }
}