using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Evaluations.EvaluationTemplates;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;

namespace Yei3.PersonalEvaluation.EvaluationTemplate
{
    public class EvaluationTemplateAppService : AsyncCrudAppService<Evaluations.EvaluationTemplates.EvaluationTemplate, EvaluationTemplateDto, long, GetAllEvaluationTemplateInputDto, EvaluationTemplateCreateInputDto>, IEvaluationTemplateAppService
    {
        public EvaluationTemplateAppService(IRepository<Evaluations.EvaluationTemplates.EvaluationTemplate, long> repository) : base(repository)
        {
        }

        protected override IQueryable<Evaluations.EvaluationTemplates.EvaluationTemplate> CreateFilteredQuery(GetAllEvaluationTemplateInputDto input)
        {
            return Repository
                .GetAll()
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                    .ThenInclude(section => section.Questions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                    .ThenInclude(section => section.ChildSections)
                    .ThenInclude(section => section.Questions);

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
                .ThenInclude(section => section.Questions)
                .Include(evaluationTemplate => evaluationTemplate.Sections)
                .ThenInclude(section => section.ChildSections)
                .ThenInclude(section => section.Questions)
                .FirstAsync(evaluationTemplate => evaluationTemplate.Id == id);
        }

        protected override EvaluationTemplateDto MapToEntityDto(Evaluations.EvaluationTemplates.EvaluationTemplate entity)
        {
            EvaluationTemplateDto evaluationTemplateDto = base.MapToEntityDto(entity);
            evaluationTemplateDto.PurgeSubSections();
            return evaluationTemplateDto;
        }
    }
}