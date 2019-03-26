using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Yei3.PersonalEvaluation.Authorization;

namespace Yei3.PersonalEvaluation
{
    [DependsOn(
        typeof(PersonalEvaluationCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class PersonalEvaluationApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<PersonalEvaluationAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(PersonalEvaluationApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
