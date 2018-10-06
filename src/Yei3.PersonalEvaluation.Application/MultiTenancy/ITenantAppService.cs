using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Yei3.PersonalEvaluation.MultiTenancy.Dto;

namespace Yei3.PersonalEvaluation.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
