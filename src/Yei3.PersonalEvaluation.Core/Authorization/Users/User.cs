using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Authorization.Users;
using Abp.Extensions;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;

namespace Yei3.PersonalEvaluation.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public virtual string EmployeeNumber { get; set; }
        public virtual string JobDescription { get; set; }
        public virtual string Area { get; set; }
        public virtual string Region { get; set; }
        public virtual string ImmediateSupervisor { get; set; }
        public virtual string SocialReason { get; set; }
        public virtual string Scholarship { get; set; }
        public virtual DateTime EntryDate { get; set; }
        public virtual DateTime? ReassignDate { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual bool IsMale { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<Evaluation> EvaluationsReceived { get; protected set; }
        [ForeignKey("ReviewerUserId")]
        public virtual ICollection<EvaluationRevision> EvaluationRevisions { get; protected set; }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
