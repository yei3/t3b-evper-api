using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Core.OrganizationUnit;
using Yei3.PersonalEvaluation.MultiTenancy;
using Yei3.PersonalEvaluation.OrganizationUnit;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Core.Evaluations.SalesObjectives
{
    public class SalesObjectivesManager : DomainService
    {
        public IAbpSession AbpSession { get; set; }
        private readonly UserManager _userManager;

        public SalesObjectivesManager (UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ImportGTObjectivesAsync(
            string employeeNumber,
            string expectedSales,
            string realSales,
            string expectedMerma,
            string realMerma,
            string expectedMoneyDiff,
            string realMoneyDiff,
            string expectedMysteryShopper,
            string realMysteryShopper,
            string expectedRotationAV,
            string realRotationAV,
            string expectedRotationTR,
            string realRotationTR,
            string expectedCostMO,
            string realMOCostMO,
            string expectedAudits,
            string realAudits,
            string expectedSO,
            string realSO
        )
        {
            using (IUnitOfWorkCompleteHandle unitOfWork = UnitOfWorkManager.Begin())
            {
                User user = await _userManager.FindByEmployeeNumberAsync(employeeNumber);

                if (user.IsNullOrDeleted())
                {
                    
                }
            }

            return true;
        }
    }
}