﻿using Abp.Domain.Entities.Auditing;

namespace Yei3.PersonalEvaluation.Evaluations.Questions
{
    public class Question : FullAuditedEntity<long>
    {
        public string Text { get; protected set; }
        public QuestionType QuestionType { get; set; }
    }
}