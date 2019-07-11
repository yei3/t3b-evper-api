using System.Linq;

using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.OrganizationUnits
{
    using OrganizationUnit;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Domain.Repositories;
    using Abp.Organizations;
    using Abp.Runtime.Session;
    using Dto;
    using Abp.AutoMapper;
    using Yei3.PersonalEvaluation.Authorization.Roles;
    using Abp.UI;
    using Yei3.PersonalEvaluation.Application.OrganizationUnits.Dto;

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
            List<OrganizationUnitDto> organizationsUnitDto = organizationUnits.MapTo<List<OrganizationUnitDto>>();

            foreach (OrganizationUnitDto organizationUnitDto in organizationsUnitDto)
            {
                var currentOrganizationUnit = organizationUnits.FirstOrDefault(organizationUnit => organizationUnit.Id == organizationUnitDto.Id);

                organizationUnitDto.OrganizationUnitUsers =
                    (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit))
                    .MapTo<ICollection<OrganizationUnitUserDto>>();
            }

            return organizationsUnitDto;
        }

        public async Task<ICollection<OrganizationUnitDto>> GetAllRegionsOrganizationUnits()
        {
            List<RegionOrganizationUnit> organizationUnits = await _regionOrganizationUnitRepository.GetAllListAsync();
            List<OrganizationUnitDto> organizationUnitDtos = organizationUnits.MapTo<List<OrganizationUnitDto>>();
            return organizationUnitDtos;
        }

        public async Task<ICollection<OrganizationUnitDto>> GetAllAreaOrganizationUnits()
        {
            List<AreaOrganizationUnit> organizationUnits = await _areaOrganizationUnitRepository.GetAllListAsync();
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

        public async Task<ICollection<OrganizationUnitDto>> GetMyRegionOrganizationUnit()
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(administratorUser);
            List<Abp.Organizations.OrganizationUnit> regions = new List<OrganizationUnit>();
            foreach (Abp.Organizations.OrganizationUnit area in organizationUnits)
            {
                if (area.ParentId != null)
                {
                    regions.Add(_regionOrganizationUnitRepository.Get(area.ParentId.Value));
                }
            }
            List<OrganizationUnitDto> organizationUnitDtos = regions.MapTo<List<OrganizationUnitDto>>();
            return organizationUnitDtos;
        }

        public async Task<ICollection<OrganizationUnitDto>> GetAreasOrganizationUnitTree()
        {
            User currentUser = await GetCurrentUserIfAdmin();

            IEnumerable<OrganizationUnitDto> areas = (await UserManager.GetOrganizationUnitsAsync(currentUser))
                .OfType<AreaOrganizationUnit>()
                .Select(organizationUnit => organizationUnit.MapTo<OrganizationUnitDto>());

            IQueryable<User> subordinates = (await UserManager.GetSubordinatesTree(currentUser));

            foreach (User subordinate in subordinates)
            {
                var subordinateAreas = (await UserManager.GetOrganizationUnitsAsync(subordinate))
                    .OfType<AreaOrganizationUnit>()
                    .Select(organizationUnit => organizationUnit.MapTo<OrganizationUnitDto>());

                areas.Concat(subordinateAreas);
            }

            return areas.Distinct().ToList();
        }

        public async Task<ICollection<UserJobDescriptionDto>> GetUserJobDescriptionTree()
        {
            User currentUser = await GetCurrentUserIfAdmin();

            List<User> subordinates = (await UserManager.GetSubordinatesTree(currentUser)).ToList();
            List<UserJobDescriptionDto> usersJobDescription = new List<UserJobDescriptionDto>();
            
            foreach (User user in subordinates)
            {
                UserJobDescriptionDto currentUserJobDescription = user.MapTo<UserJobDescriptionDto>();
                currentUserJobDescription.AreaIds = (await UserManager.GetOrganizationUnitsAsync(user))
                    .OfType<AreaOrganizationUnit>()
                    .Select(organizationUnit => organizationUnit.Id)
                    .ToList();
                    
                usersJobDescription.Add(currentUserJobDescription);
            }

            return usersJobDescription;
        }

        private async Task<User> GetCurrentUserIfAdmin()
        {
            User currentUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (!await UserManager.IsInRoleAsync(currentUser, StaticRoleNames.Tenants.Administrator))
            {
                throw new UserFriendlyException(403, $"Usuario {currentUser.FullName} no tiene autorizacion.");
            }

            return currentUser;
        }

        public async Task<ICollection<UserFullNameDto>> GetUserTree()
        {
            User currentUser = await GetCurrentUserIfAdmin();
            IQueryable<User> subordinates = (await UserManager.GetSubordinatesTree(currentUser));

            return subordinates.MapTo<List<UserFullNameDto>>();
        }

        public async Task<ICollection<OrganizationUnitDto>> GetRegionsOrganizationUnitTree()
        {
            User currentUser = await GetCurrentUserIfAdmin();

            IQueryable<User> subordinates = (await UserManager.GetSubordinatesTree(currentUser));

            List<OrganizationUnitDto> regions = new List<OrganizationUnitDto>();

            foreach (User subordinate in subordinates)
            {
                IEnumerable<string> regionCodes = (await UserManager.GetOrganizationUnitsAsync(subordinate))
                    .Select(organizationUnit => organizationUnit.Code.Substring(0, 5))
                    .Distinct();


                foreach (string code in regionCodes)
                {
                    OrganizationUnitDto currentRegion = (await _regionOrganizationUnitRepository
                        .SingleAsync(region => region.Code == code))
                        .MapTo<OrganizationUnitDto>();

                    if (regions.Contains(currentRegion))
                        continue;

                    regions.Add(currentRegion);
                }
            }

            return regions;
        }
    }
}