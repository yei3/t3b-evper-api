using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;
using Yei3.PersonalEvaluation.Section.Dto;

namespace Yei3.PersonalEvaluation.Section
{
    public class SectionAppService : AsyncCrudAppService<Evaluations.Sections.Section, SectionDto, long, SectionGetAllInputDto, SectionCreateInputDto>, ISectionAppService
    {
        public SectionAppService(IRepository<Evaluations.Sections.Section, long> repository) : base(repository)
        {
        }

        protected override IQueryable<Evaluations.Sections.Section> CreateFilteredQuery(SectionGetAllInputDto input)
        {
            return Repository.GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .Include(section => section.UnmeasuredQuestions)
                .Where(section => !section.ParentId.HasValue);
        }

        protected override async Task<Evaluations.Sections.Section> GetEntityByIdAsync(long id)
        {
            return await Repository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .Include(section => section.UnmeasuredQuestions)
                .FirstOrDefaultAsync(section => section.Id == id);
        }

        public override async Task Delete(EntityDto <long> input){
            await base.Delete(input);
            await Repository
                .DeleteAsync(section => section.ParentId == input.Id);
        }
    }
}