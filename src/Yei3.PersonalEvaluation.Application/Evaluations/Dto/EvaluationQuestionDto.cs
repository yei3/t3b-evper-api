using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    [AutoMap(typeof(EvaluationMeasuredQuestion), typeof(EvaluationUnmeasuredQuestion))]
    public class EvaluationQuestionDto : EntityDto<long>
    {
        public EvaluationAnswerDto MeasuredAnswer { get; set; }
        public EvaluationAnswerDto UnmeasuredAnswer { get; set; }
        public long EvaluationQuestionId { get; set; }
        public DateTime TerminationDateTime { get; set; }
        public EvaluationQuestionStatus Status { get; set; }
        public bool IsActive { get; set; }
    }
}