using System.Linq;

namespace Yei3.PersonalEvaluation.OrganizationUnits
{
    using OrganizationUnit;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Repositories;
    using Abp.Organizations;
    using Dto;
    using Abp.AutoMapper;

    public class OrganizationUnitAppService : PersonalEvaluationAppServiceBase, IOrganizationUnitAppService
    {
        private readonly IRepository<OrganizationUnit, long> _repository;
        private readonly IRepository<AreaOrganizationUnit, long> _areaOrganizationUnitRepository;
        private readonly IRepository<RegionOrganizationUnit, long> _regionOrganizationUnitRepository;

        public OrganizationUnitAppService(IRepository<OrganizationUnit, long> repository, IRepository<AreaOrganizationUnit, long> areaOrganizationUnitRepository, IRepository<RegionOrganizationUnit, long> regionOrganizationUnitRepository)
        {
            _repository = repository;
            _areaOrganizationUnitRepository = areaOrganizationUnitRepository;
            _regionOrganizationUnitRepository = regionOrganizationUnitRepository;
        }

        public async Task<ICollection<OrganizationUnitDto>> GetAllOrganizationUnits()
        {
            List<OrganizationUnit> organizationUnits = await _repository.GetAllListAsync();
            List<OrganizationUnitDto> organizationUnitDtos = organizationUnits.MapTo<List<OrganizationUnitDto>>();
            return organizationUnitDtos;
        }

        public async Task<ICollection<OrganizationUnitDto>> GetAllRegionsOrganizationUnits()
        {
            List<RegionOrganizationUnit> organizationUnits = await _regionOrganizationUnitRepository.GetAllListAsync();
            List<OrganizationUnitDto> organizationUnitDtos = organizationUnits.MapTo<List<OrganizationUnitDto>>();
            return organizationUnitDtos;
        }

        public async Task<ICollection<OrganizationUnitDto>> GetAllAreasOrganizationUnitsByRegionCode(string regionCode)
        {
            List<AreaOrganizationUnit> organizationUnits = (await _areaOrganizationUnitRepository
                .GetAllListAsync())
                .Where(organizationUnit => organizationUnit.Code.StartsWith(regionCode))
                .ToList();
            List<OrganizationUnitDto> organizationUnitDtos = organizationUnits.MapTo<List<OrganizationUnitDto>>();
            return organizationUnitDtos;
        }
    }
}