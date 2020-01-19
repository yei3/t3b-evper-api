﻿using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Binnacle
{
    public class ObjectiveBinnacle : FullAuditedEntity<long>
    {
        public ObjectiveBinnacle(string text, long evaluationQuestionId)
        {
            Text = text;
            EvaluationQuestionId = evaluationQuestionId;
        }

        public ObjectiveBinnacle(string text, long evaluationQuestionId, long? creatorUserId)
        {
            Text = text;
            EvaluationQuestionId = evaluationQuestionId;
            CreatorUserId = creatorUserId;
        }

        public virtual string Text { get; protected set; }
        public virtual long EvaluationQuestionId { get; protected set; }

        [ForeignKey("EvaluationQuestionId")]
        public virtual EvaluationQuestion EvaluationQuestion { get; protected set; }
    }
}