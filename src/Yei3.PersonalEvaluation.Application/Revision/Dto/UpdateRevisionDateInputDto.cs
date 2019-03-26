using System;
using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Revision.Dto
{
    public class UpdateRevisionDateInputDto
    {
        public long EvaluationId { get; set; }
        public DateTime RevisionTime { get; set; }
    }
}