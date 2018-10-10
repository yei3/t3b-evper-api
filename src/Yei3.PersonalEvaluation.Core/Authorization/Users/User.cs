﻿using System;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace Yei3.PersonalEvaluation.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";
        public const int EmployeeNumberLength = 4;

        public virtual string EmployeeNumber { get; protected set; }

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
