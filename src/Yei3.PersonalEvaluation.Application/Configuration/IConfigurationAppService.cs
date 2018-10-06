using System.Threading.Tasks;
using Yei3.PersonalEvaluation.Configuration.Dto;

namespace Yei3.PersonalEvaluation.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
