namespace Yei3.PersonalEvaluation.DataAdquisition
{
    using Abp.Modules;
    using Abp.Reflection.Extensions;
    using Abp.Zero.Configuration;
    using Castle.Windsor.MsDependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;
    using Yei3.PersonalEvaluation.Authorization.Roles;
    using Yei3.PersonalEvaluation.Authorization.Users;
    using Yei3.PersonalEvaluation.Configuration;
    using Yei3.PersonalEvaluation.EntityFrameworkCore;
    using Yei3.PersonalEvaluation.Identity;
    using Yei3.PersonalEvaluation.MultiTenancy;

    [DependsOn(typeof(PersonalEvaluationEntityFrameworkModule))]
    public class PersonalEvaluationDataAdquisitionModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PersonalEvaluationDataAdquisitionModule (PersonalEvaluationEntityFrameworkModule personalEvaluationEntityFrameworkModule)
        {
            _appConfiguration = AppConfigurations.Get(
                typeof(PersonalEvaluationDataAdquisitionModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                PersonalEvaluationConsts.ConnectionStringName
            );

            Configuration.MultiTenancy.IsEnabled = true;

            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
        }

        public override void Initialize()
        {
            var services = new ServiceCollection();
            IdentityRegistrar.Register(services);
            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}