using System.IO;
using System.Net.Mail;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Net.Mail;
using Castle.Core.Logging;
using RazorLight;

namespace Yei3.PersonalEvaluation.Core.Authorization.Users.BackgroundJob
{
    public class SendImportUserReportBackgroundJob : BackgroundJob<ImportUserSummaryModel>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public SendImportUserReportBackgroundJob(IEmailSender emailSender, ILogger logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public override void Execute(ImportUserSummaryModel args)
        {
            _logger.Info($"Sending user import email to {args.EmailAddress}");

            var engine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();

            string template = engine.CompileRenderAsync("ImporUserSummary",
                    File.ReadAllText(args.TemplatePath, System.Text.Encoding.UTF8), args)
                .GetAwaiter()
                .GetResult();
            
            _logger.Info("User import report parsed");

            MailMessage mail = new MailMessage(
                new MailAddress("noreply@t3b.mx", "Tiendas 3B"),
                new MailAddress(args.EmailAddress)
            );

            mail.Body = template;
            mail.Subject = $"Reporte de importacion de usuarios";

            _emailSender.Send(mail);
            _logger.Info("User import report email sent");
        }
    }
}