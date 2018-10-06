using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Yei3.PersonalEvaluation.Localization
{
    public static class PersonalEvaluationLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(PersonalEvaluationConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(PersonalEvaluationLocalizationConfigurer).GetAssembly(),
                        "Yei3.PersonalEvaluation.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
