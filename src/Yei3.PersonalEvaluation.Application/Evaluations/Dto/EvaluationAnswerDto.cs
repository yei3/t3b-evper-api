using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    [AutoMap(typeof(MeasuredAnswer), typeof(UnmeasuredAnswer), typeof(NotEvaluableAnswer))]
    public class EvaluationAnswerDto : EntityDto<long>
    {
        public decimal Real { get; set; }
        public string Action { get; set; }
        public DateTime CommitmentDate { get; set; }
        public DateTime CommitmentTime { get; set; }
        public string Text { get; set; }
        public string Observations { get; set; }
        public EvaluationMeasuredQuestionDto EvaluationMeasuredQuestion { get; set; }
    }
}