using Abp.AutoMapper;

namespace Yei3.PersonalEvaluation.OrganizationUnits
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Repositories;
    using Abp.Organizations;
    using Dto;

    public class OrganizationUnitAppService : PersonalEvaluationAppServiceBase, IOrganizationUnitAppService
    {
        private readonly IRepository<OrganizationUnit, long> _repository;

        public OrganizationUnitAppService(IRepository<OrganizationUnit, long> repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<OrganizationUnitDto>> GetAllOrganizationUnits()
        {
            List<OrganizationUnit> organizationUnits = _repository.GetAllList();
            List<OrganizationUnitDto> organizationUnitDtos = organizationUnits.MapTo<List<OrganizationUnitDto>>();
            return organizationUnitDtos;
        }
    }
}