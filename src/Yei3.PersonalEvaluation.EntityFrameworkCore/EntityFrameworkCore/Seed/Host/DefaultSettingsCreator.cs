using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly PersonalEvaluationDbContext _context;

        public DefaultSettingsCreator(PersonalEvaluationDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            // Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "comunicadosrh@t3b.com.mx");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "Soporte Tiendas 3B");
            AddSettingIfNotExists("SendGridKey.Key", "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");

            // Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "es-Mx");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}
