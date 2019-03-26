using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Objective.Dto;

namespace Yei3.PersonalEvaluation.EvaluationObjectives.Dto
{
    [AutoMap(typeof(MeasuredAnswer))]
    public class EvaluationObjectiveDto : EntityDto<long>
    {
        public string Text { get; set; }
        public decimal Real { get; set; }
        public long EvaluationQuestionId { get; set; }
        public bool IsActive { get; set; }
        public EvaluationMeasuredQuestionStatusDto EvaluationMeasuredQuestion { get; set; }
        public string Observations { get; set; }
    }
}