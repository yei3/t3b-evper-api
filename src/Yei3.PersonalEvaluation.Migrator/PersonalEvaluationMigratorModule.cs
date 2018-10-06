using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Yei3.PersonalEvaluation.Configuration;
using Yei3.PersonalEvaluation.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Migrator.DependencyInjection;

namespace Yei3.PersonalEvaluation.Migrator
{
    [DependsOn(typeof(PersonalEvaluationEntityFrameworkModule))]
    public class PersonalEvaluationMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PersonalEvaluationMigratorModule(PersonalEvaluationEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(PersonalEvaluationMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                PersonalEvaluationConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PersonalEvaluationMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
