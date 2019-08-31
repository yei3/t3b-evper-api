using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Yei3.PersonalEvaluation.Core.Email
{
    public class SendGridEmailSender : EmailSenderBase
    {
        private readonly ISettingManager _settingManager;

        public SendGridEmailSender(ISettingManager settingManager, IEmailSenderConfiguration emailSenderConfiguration)
        : base(emailSenderConfiguration)
        {
            _settingManager = settingManager;
        }

        protected override void SendEmail(MailMessage mail)
        {
            SendGridMessage sendGridMessage;

            if (mail.IsBodyHtml)
            {
                sendGridMessage = MailHelper.CreateSingleEmailToMultipleRecipients(
                    new EmailAddress(mail.From.Address),
                    mail.To.Select(mailAddress => new EmailAddress(mailAddress.Address)).ToList(),
                    mail.Subject,
                    string.Empty,
                    mail.Body
                );
            }
            else
            {
                sendGridMessage = MailHelper.CreateSingleEmailToMultipleRecipients(
                    new EmailAddress(mail.From.Address),
                    mail.To.Select(mailAddress => new EmailAddress(mailAddress.Address)).ToList(),
                    mail.Subject,
                    mail.Body,
                    string.Empty
                );
            }

            SendGridClient sendGridClient = new SendGridClient(_settingManager.GetSettingValue("SendGridKey.Key"));
            sendGridClient.SendEmailAsync(sendGridMessage).GetAwaiter().GetResult();
        }

        protected async override Task SendEmailAsync(MailMessage mail)
        {
            SendGridMessage sendGridMessage;

            if (mail.IsBodyHtml)
            {
                sendGridMessage = MailHelper.CreateSingleEmailToMultipleRecipients(
                    new EmailAddress(mail.From.Address),
                    mail.To.Select(mailAddress => new EmailAddress(mailAddress.Address)).ToList(),
                    mail.Subject,
                    string.Empty,
                    mail.Body
                );
            }
            else
            {
                sendGridMessage = MailHelper.CreateSingleEmailToMultipleRecipients(
                    new EmailAddress(mail.From.Address),
                    mail.To.Select(mailAddress => new EmailAddress(mailAddress.Address)).ToList(),
                    mail.Subject,
                    mail.Body,
                    string.Empty
                );
            }

            SendGridClient sendGridClient = new SendGridClient(await _settingManager.GetSettingValueAsync("SendGridKey.Key"));
            await sendGridClient.SendEmailAsync(sendGridMessage);
        }
    }
}