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
        public DateTime CommitmentTime { get; set; }
        public string Text { get; set; }
    }
}