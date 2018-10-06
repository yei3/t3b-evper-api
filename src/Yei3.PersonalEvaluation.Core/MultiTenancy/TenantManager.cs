using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Editions;

namespace Yei3.PersonalEvaluation.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore)
        {
        }
    }
}
