using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Evaluations.EvaluationTemplates;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.Interfaces;

namespace Yei3.PersonalEvaluation.Evaluations.Sections
{
    public class Section : FullAuditedEntity<long>, INamed, IPassivable
    {
        public Section(string name, bool displayName, long evaluationTemplateId, long? parentId, bool isActive)
        {
            Name = name;
            DisplayName = displayName;
            EvaluationTemplateId = evaluationTemplateId;
            ParentId = parentId;
            IsActive = isActive;
            ChildSections = new List<Section>();
            UnmeasuredQuestions = new List<UnmeasuredQuestion>();
            MeasuredQuestions = new List<MeasuredQuestion>();
        }

        public virtual string Name { get; protected set; }
        public virtual bool DisplayName { get; protected set; }
        public virtual long EvaluationTemplateId { get; protected set; }

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

        public Section NoTracking(long evaluationId, bool trackParent = false)
        {
            Section noTrackedSection = new Section(
                Name,
                DisplayName,
                evaluationId,
                ParentId,
                IsActive);

            if (trackParent)
            {
                noTrackedSection.ParentSection = ParentSection?.NoTracking(evaluationId);
            }

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

            foreach (UnmeasuredQuestion unmeasuredQuestion in UnmeasuredQuestions)
            {
                noTrackedSection.UnmeasuredQuestions.Add(new UnmeasuredQuestion(
                    unmeasuredQuestion.Text,
                    unmeasuredQuestion.QuestionType,
                    unmeasuredQuestion.IsQualifiable));
            }

            foreach (Section childSection in ChildSections)
            {
                noTrackedSection.ChildSections.Add(childSection.NoTracking(evaluationId));
            }

            return noTrackedSection;

        }
    }
}