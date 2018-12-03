﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.OrganizationUnits.Dto;

namespace Yei3.PersonalEvaluation.OrganizationUnits
{
    public interface IOrganizationUnitAppService : IApplicationService
    {
        Task<ICollection<OrganizationUnitDto>> GetAllOrganizationUnits();
        Task<ICollection<OrganizationUnitDto>> GetAllRegionsOrganizationUnits();
        Task<ICollection<OrganizationUnitDto>> GetAllAreaOrganizationUnits();
        Task<ICollection<OrganizationUnitDto>> GetAllAreasOrganizationUnitsByRegionCode(string regionCode);
    }
}