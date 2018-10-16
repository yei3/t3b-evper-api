namespace Yei3.PersonalEvaluation.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string Admin = "Admin"; // Admin stands for system admin
            public const string Administrator = "Admnistrator";
            public const string Supervisor = "Supervisor";
            public const string Collaborator = "Collaborator";
        }
    }
}
