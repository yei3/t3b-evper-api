using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;

namespace Yei3.PersonalEvaluation.NotEvaluableAnswers.Dto
{
    [AutoMap(typeof(NotEvaluableAnswer))]
    public class NotEvaluableAnswerDto : EntityDto<long>
    {
        public long EvaluationQuestionId { get; set; }
        public DateTime CommitmentTime { get; set; }
        public string Text { get; set; }
    }
}