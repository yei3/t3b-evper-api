using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.EvaluationTemplates;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.Interfaces;

namespace Yei3.PersonalEvaluation.Evaluations.Sections
{
    public class Section : FullAuditedEntity<long>, INamed, IPassivable
    {
        public Section()
        {
        }

        public Section(string name, bool displayName, long evaluationTemplateId, long? parentId, bool isActive, float value)
        {
            Name = name;
            DisplayName = displayName;
            EvaluationTemplateId = evaluationTemplateId;
            ParentId = parentId;
            IsActive = isActive;
            ChildSections = new List<Section>();
            UnmeasuredQuestions = new List<UnmeasuredQuestion>();
            MeasuredQuestions = new List<MeasuredQuestion>();
            Value = value;
        }

        public virtual string Name { get; set; }
        public virtual bool DisplayName { get; protected set; }
        public virtual long EvaluationTemplateId { get; protected set; }
        public virtual float Value { get; set; }

        [ForeignKey("EvaluationTemplateId")]
        public virtual EvaluationTemplate Template { get; protected set; }
        public virtual long? ParentId { get; protected set; }
        [ForeignKey("ParentId")]
        [InverseProperty(nameof(ChildSections))]
        public virtual Section ParentSection { get; protected set; }
        public virtual ICollection<Section> ChildSections { get; protected set; }

        public virtual bool IsActive { get; set; }
        public virtual ICollection<UnmeasuredQuestion> UnmeasuredQuestions { get; protected set; }
        public virtual ICollection<MeasuredQuestion> MeasuredQuestions { get; protected set; }
        public virtual ICollection<NotEvaluableQuestion> NotEvaluableQuestions { get; protected set; }

        public Section NoTracking(long sourceTemplateId, long sourceEvaluationId, long destinyTemplateId, long destinyEvaluationId, bool trackParent = false)
        {
            Section noTrackedSection = new Section(
                Name,
                DisplayName,
                destinyTemplateId,
                ParentId,
                IsActive,
                Value);

            if (trackParent)
            {
                noTrackedSection.ParentSection = ParentSection?.NoTracking(sourceTemplateId, sourceEvaluationId, destinyTemplateId, destinyEvaluationId);
            }

            if (!MeasuredQuestions.IsNullOrEmpty())
            {
                noTrackedSection.MeasuredQuestions = new List<MeasuredQuestion>();

                foreach (MeasuredQuestion measuredQuestion in MeasuredQuestions)
                {
                    noTrackedSection.MeasuredQuestions.Add(new MeasuredQuestion(
                        measuredQuestion.Text,
                        measuredQuestion.QuestionType,
                        measuredQuestion.IsQualifiable,
                        measuredQuestion.Expected,
                        measuredQuestion.ExpectedText,
                        measuredQuestion.Relation,
                        measuredQuestion.Deliverable));
                }
            }

            if (!UnmeasuredQuestions.IsNullOrEmpty())
            {
                noTrackedSection.UnmeasuredQuestions = new List<UnmeasuredQuestion>();

                foreach (UnmeasuredQuestion unmeasuredQuestion in UnmeasuredQuestions)
                {
                    noTrackedSection.UnmeasuredQuestions.Add(new UnmeasuredQuestion(
                        unmeasuredQuestion.Text,
                        unmeasuredQuestion.QuestionType,
                        unmeasuredQuestion.IsQualifiable));
                }
            }

            if (!NotEvaluableQuestions.IsNullOrEmpty())
            {
                noTrackedSection.NotEvaluableQuestions = new List<NotEvaluableQuestion>();

                foreach (NotEvaluableQuestion question in NotEvaluableQuestions.Where(question => question.EvaluationId == sourceEvaluationId))
                {

                    NotEvaluableQuestion currentQuestion = new NotEvaluableQuestion(
                        noTrackedSection.Id,
                        question.Text,
                        destinyEvaluationId,
                        question.TerminationDateTime,
                        question.Status
                    );

                    currentQuestion.SetAnswer(
                        currentQuestion.Id,
                        question.NotEvaluableAnswer.Text,
                        question.NotEvaluableAnswer.CommitmentTime
                    );

                    noTrackedSection.NotEvaluableQuestions.Add(currentQuestion);
                }
            }

            if (ChildSections.IsNullOrEmpty()) return noTrackedSection;
            foreach (Section childSection in ChildSections)
            {
                noTrackedSection.ChildSections.Add(childSection.NoTracking(sourceTemplateId, sourceEvaluationId, destinyTemplateId, destinyEvaluationId));
            }

            return noTrackedSection;

        }
    }
}