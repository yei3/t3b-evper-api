using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;

namespace Yei3.PersonalEvaluation.EvaluationTemplate
{
    public class EvaluationTemplateAppService : AsyncCrudAppService<Evaluations.EvaluationTemplates.EvaluationTemplate, EvaluationTemplateDto, long, GetAllEvaluationTemplateInputDto, EvaluationTemplateCreateInputDto>, IEvaluationTemplateAppService
    {
        private readonly IRepository<Evaluations.Sections.Section, long> SectionRepository;
        public EvaluationTemplateAppService(IRepository<Evaluations.EvaluationTemplates.EvaluationTemplate, long> repository, IRepository<Evaluations.Sections.Section, long> sectionRepository) : base(repository)
        {
            SectionRepository = sectionRepository;
        }

        protected override IQueryable<Evaluations.EvaluationTemplates.EvaluationTemplate> CreateFilteredQuery(GetAllEvaluationTemplateInputDto input)
        {
            return Repository
                .GetAll()
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                    .ThenInclude(section => section.UnmeasuredQuestions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                    .ThenInclude(section => section.MeasuredQuestions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                    .ThenInclude(section => section.ChildSections)
                    .ThenInclude(section => section.MeasuredQuestions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                    .ThenInclude(section => section.ChildSections)
                    .ThenInclude(section => section.UnmeasuredQuestions);

        }

        public override async Task<PagedResultDto<EvaluationTemplateDto>> GetAll(GetAllEvaluationTemplateInputDto input)
        {
            var result = await base.GetAll(input);

            return result;
        }

        protected override Task<Evaluations.EvaluationTemplates.EvaluationTemplate> GetEntityByIdAsync(long id)
        {
            return Repository
                .GetAll()
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                .ThenInclude(section => section.MeasuredQuestions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                .ThenInclude(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                .ThenInclude(section => section.ChildSections)
                .ThenInclude(section => section.MeasuredQuestions)
                .FirstAsync(evaluationTemplate => evaluationTemplate.Id == id);
        }

        protected override EvaluationTemplateDto MapToEntityDto(Evaluations.EvaluationTemplates.EvaluationTemplate entity)
        {
            EvaluationTemplateDto evaluationTemplateDto = base.MapToEntityDto(entity);
            evaluationTemplateDto.PurgeSubSections();
            return evaluationTemplateDto;
        }

        public override Task Delete(EntityDto<long> input)
        {
            Repository
                .GetAll()
                .Include(template => template.Evaluations)
                .Single(template => template.Id == input.Id)
                .Evaluations.Clear();

            return base.Delete(input);
        }

        public override async Task<EvaluationTemplateDto> Create(EvaluationTemplateCreateInputDto input)
        {
            Evaluations.EvaluationTemplates.EvaluationTemplate lastEvaluationTemplate = Repository
                .GetAll()
                .OrderByDescending(evaluationTemplate => evaluationTemplate.CreationTime)
                .FirstOrDefault();

            EvaluationTemplateDto result = await base.Create(input);

            await CurrentUnitOfWork.SaveChangesAsync();

            if (lastEvaluationTemplate.IsNullOrDeleted()) return result;

            Evaluations.Sections.Section nextObjectivesParentSection = SectionRepository
                .GetAll()
                .Where(section => section.EvaluationTemplateId == lastEvaluationTemplate.Id)
                .Include(section => section.UnmeasuredQuestions)
                .Include(section => section.MeasuredQuestions)
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.MeasuredQuestions)
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .FirstOrDefault(section =>
                    section.Name.StartsWith(AppConsts.SectionNextObjectives,
                        StringComparison.CurrentCultureIgnoreCase));

            nextObjectivesParentSection = nextObjectivesParentSection?.NoTracking(result.Id, true);

            if (nextObjectivesParentSection.IsNullOrDeleted()) return result;

            Repository
                .GetAll()
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                .Single(evaluationTemplate => evaluationTemplate.Id == result.Id)
                .Sections
                .Add(nextObjectivesParentSection);

            return result;
        }
    }
}