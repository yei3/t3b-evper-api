using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;
using Abp.AutoMapper;
using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    [AutoMap(typeof(EvaluationRevision))]
    public class EvaluationRevisionDto : EntityDto<long>
    {

    }
}