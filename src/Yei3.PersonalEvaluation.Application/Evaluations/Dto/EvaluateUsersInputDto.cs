namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using System.Collections.Generic;

    public class EvaluateUsersInputDto
    {
        public long EvaluationId { get; set; }
        public ICollection<long> EvaluatedUserIds { get; set; }
    }
}