using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.Questions;

namespace Yei3.PersonalEvaluation.EvaluationTemplate.Dto
{
    [AutoMap(typeof(UnmeasuredQuestion), typeof(MeasuredQuestion))]
    public class QuestionDto : EntityDto<long>
    {
        public string Text { get; set; }
        public QuestionType QuestionType { get; set; }
        public long SectionId { get; set; }
        public decimal? Expected { get; set; } 
        public string Deliverable { get; set; } 
    }
}