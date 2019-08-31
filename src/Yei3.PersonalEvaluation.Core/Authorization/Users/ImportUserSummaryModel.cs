using System.Collections.Generic;

namespace Yei3.PersonalEvaluation.Core.Authorization.Users
{
    public class ImportUserSummaryModel
    {
        public ImportUserSummaryModel(string templatePath, string emailAddress)
        {
            ImportedUsers = 0;
            NotImportedUsers = 0;
            ImportedUserDictionary = new Dictionary<string, string>();
            TemplatePath = templatePath;
            EmailAddress = emailAddress;
        }

        public int ImportedUsers { get; protected set; }
        public int NotImportedUsers { get; protected set; }
        public Dictionary<string, string> ImportedUserDictionary { get; protected set; }
        public string TemplatePath { get; protected set; }
        public string EmailAddress { get; protected set; }

        public void IncrementImportedUser() {
            ImportedUsers ++;
        }

        public void IncrementNotImportedUser() {
            NotImportedUsers ++;
        }
    }
}