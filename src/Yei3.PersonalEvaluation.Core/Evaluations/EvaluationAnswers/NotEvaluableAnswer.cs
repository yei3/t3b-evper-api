﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public class NotEvaluableAnswer : Answer
    {
        public NotEvaluableAnswer()
        {
        }

        public NotEvaluableAnswer(long evaluationQuestionId) : base(evaluationQuestionId)
        {
            
        }

        [ForeignKey("EvaluationQuestionId")]
        public virtual NotEvaluableQuestion NotEvaluableQuestion { get; protected set; }
        public virtual DateTime CommitmentTime { get; protected set; }
    }
}