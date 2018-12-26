using System;
using Abp.Application.Services.Dto;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class EvaluationResultsDto : EntityDto<long>
    {
        public long EvaluationTemplateId { get; set; }
        public EvaluationStatus Status { get; set; }
        public EvaluationTerm Term { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Total { get; set; }
        public int Finished { get; set; }
        public DateTime CreationTime { get; set; }
    }
}