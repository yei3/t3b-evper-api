using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Yei3.PersonalEvaluation.Configuration;

namespace Yei3.PersonalEvaluation.Web.Host.Startup
{
    [DependsOn(
       typeof(PersonalEvaluationWebCoreModule))]
    public class PersonalEvaluationWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public PersonalEvaluationWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PersonalEvaluationWebHostModule).GetAssembly());
        }
    }
}
