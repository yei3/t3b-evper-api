using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
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
            //Off course is not a good solution but i don't find any other yet
            input.MaxResultCount = 100;
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
            EvaluationTemplateDto result = await base.Create(input);

            await CurrentUnitOfWork.SaveChangesAsync();

            if (!result.IncludePastObjectives) return result;

            long nextObjectivesSectionId = await SectionRepository.InsertAndGetIdAsync(new Evaluations.Sections.Section(AppConsts.SectionNextObjectivesName, true, result.Id, null, true, 0));
            await SectionRepository.InsertAsync(new Evaluations.Sections.Section(AppConsts.SectionObjectivesName, false, result.Id,
                nextObjectivesSectionId, true, 0));

            long objectiveSectionId = await SectionRepository.InsertAndGetIdAsync(new Evaluations.Sections.Section(AppConsts.SectionObjectivesName, true, result.Id, null, true, 0));
            await SectionRepository.InsertAsync(new Evaluations.Sections.Section(AppConsts.SectionObjectivesName, false, result.Id,
                objectiveSectionId, true, 0));

            return result;
        }
    }
}