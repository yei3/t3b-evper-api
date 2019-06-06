using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using Abp;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Runtime.Session;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Sessions;
using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Yei3.PersonalEvaluation.Evaluations;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Yei3.PersonalEvaluation.Notifications
{
    public class NotificationsAppService : ApplicationService, INotificationsAppService
    {
        private readonly INotificationPublisher _notiticationPublisher;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IUserNotificationManager  _userNotificationManager;

        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly ISessionAppService _sessionAppService;
        private readonly UserManager UserManager;

        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitRepository;

         public NotificationsAppService(INotificationSubscriptionManager notificationSubscriptionManager, UserManager userManager, INotificationPublisher notiticationPublisher, IUserNotificationManager  userNotificationManager, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitRepository, IRepository<Evaluation, long> evaluationRepository)
        {
            _notificationSubscriptionManager = notificationSubscriptionManager;
            UserManager = userManager;
            _notiticationPublisher = notiticationPublisher;
            _userNotificationManager = userNotificationManager;
            OrganizationUnitRepository = organizationUnitRepository;
            EvaluationRepository = evaluationRepository;
        }

        public async Task<List<UserNotification>> getAll()
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            return await _userNotificationManager.GetUserNotificationsAsync(new Abp.UserIdentifier(administratorUser.TenantId, administratorUser.Id), 0, 0, 100);
        }

        public async Task<int> getNotifCount()
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            return await _userNotificationManager.GetUserNotificationCountAsync(new Abp.UserIdentifier(administratorUser.TenantId, administratorUser.Id),0);
        }

        public async Task Publish_SentGeneralUserNotification(string senderUserName, string generalMessage)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            UserIdentifier targetUserId = new UserIdentifier(administratorUser.TenantId, administratorUser.Id);
            await _notiticationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData(senderUserName, generalMessage), userIds: new[] { targetUserId });
        }

        public async Task Publish_SentGeneralMultipleUserNotification(CreateNotificationDto input)
        {
            if(string.IsNullOrEmpty(input.GeneralMessage)){
                throw new UserFriendlyException($"Por favor ingrese un mensaje.");
            }
            if(input.JobDescriptions.IsNullOrEmpty<string>() && input.UserIds.IsNullOrEmpty<long>() && input.OrganizationUnitIds.IsNullOrEmpty<long>()){
                throw new UserFriendlyException($"Debe seleccionar algún destinatario para el mensaje.");
            }
            List<User> users = await UserManager
                .Users
                .Where(user => input.UserIds.Contains(user.Id) ) 
                .ToListAsync();

            foreach (long inputOrganizationUnitId in input.OrganizationUnitIds)
            {
                Abp.Organizations.OrganizationUnit currentOrganizationUnit = await
                    OrganizationUnitRepository.FirstOrDefaultAsync(inputOrganizationUnitId);

                List<User> allUsers = await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true);

                users.AddRange(
                    allUsers
                        /* .Where(user => UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Collaborator).GetAwaiter().GetResult()
                        )*/
                    );
            }

            if(!input.JobDescriptions.IsNullOrEmpty<string>()){
                List<User> usersJob = await UserManager
                .Users
                .Where(user => input.JobDescriptions.Contains(user.JobDescription) ) 
                .ToListAsync();

                users.AddRange(
                    usersJob
                );

            }

            await _notiticationPublisher.PublishAsync("GeneralNotification", 
                        new SentGeneralUserNotificationData(input.SenderName, input.GeneralMessage), 
                        userIds: users.Select(user => new UserIdentifier(user.TenantId, user.Id)).ToArray());
        }

        public async Task Publish_SentBossGeneralUserNotification(long objectiveId)
        {
            
            User userLogged = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            if(!string.IsNullOrEmpty(userLogged.ImmediateSupervisor))
            {
                List<User> users = await UserManager
                .Users
                .Where(user => userLogged.ImmediateSupervisor.Equals(user.JobDescription) ) 
                .ToListAsync();
                if(!users.IsNullOrEmpty<User>())
                {
                    User bossUser = users[0];
                    UserIdentifier targetUserId = new UserIdentifier(bossUser.TenantId, bossUser.Id);
                    await _notiticationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData("Administrador", "El evaluado "+userLogged.FullName+" ha completado un objetivo."), userIds: new[] { targetUserId });
                }
                
            }

            
        }

        public async Task Publish_SentReviewNotification(SentReviewNotificationData input)
        {
            User supervisor = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            Evaluation evaluation = EvaluationRepository.FirstOrDefault(input.EvaluationId);
            User collaborator = await UserManager
            .GetUserByIdAsync(evaluation.UserId);
            UserIdentifier targetUserId = new UserIdentifier(supervisor.TenantId, collaborator.Id);
            string dateReview = input.DateReview;
            await _notiticationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData(supervisor.FullName, "Se agendó la revisión de tu evaluación en la fecha " + dateReview), userIds: new[] { targetUserId });

            // Temporary solution the key must be in the appsettings
            var sendGridClient = new SendGridClient("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");
            var from = new EmailAddress("comunicadosrh@t3b.com.mx", "Soporte Tiendas 3B");
            var subject = "Revisión de evaluación - Evaluación de desempeño";
            var to = new EmailAddress(collaborator.EmailAddress, collaborator.FullName);
            var plainTextContent = $"Se agendó la revisión de tu evaluación en la siguiente fecha:  {dateReview}.";
            // We need create a email template
            var htmlContent = $"Hola {collaborator.Name} <br/>Se agendó la revisión de tu evaluación en la siguiente fecha: <strong>{dateReview}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await sendGridClient.SendEmailAsync(msg);

        }

        public async Task Publish_SendBossCloseEvaluationNotification()
        {
            User userLogged = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            if(!string.IsNullOrEmpty(userLogged.ImmediateSupervisor))
            {
                List<User> users = await UserManager
                .Users
                .Where(user => userLogged.ImmediateSupervisor.Equals(user.JobDescription) ) 
                .ToListAsync();
                if(!users.IsNullOrEmpty<User>())
                {
                    User bossUser = users[0];
                    UserIdentifier targetUserId = new UserIdentifier(bossUser.TenantId, bossUser.Id);
                    await _notiticationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData("Cierre de evaluación.", "El evaluado "+userLogged.FullName+" ha concluido el proceso de evaluación."), userIds: new[] { targetUserId });
                }   
            }
        }

        public async Task Publish_SendValidateEvaluationNotification(SentReviewNotificationData input)
        {
            User supervisor = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            Evaluation evaluation = EvaluationRepository.FirstOrDefault(input.EvaluationId);
            User collaborator = await UserManager
            .GetUserByIdAsync(evaluation.UserId);
            UserIdentifier targetUserId = new UserIdentifier(supervisor.TenantId, collaborator.Id);
            await _notiticationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData("Cierre de evaluación", "Se validó el cierre de tu evaluación."), userIds: new[] { targetUserId });
        }

        public async Task Publish_SentGeneralMessageToAdminMail(CreateNotificationDto input)
        {
            if(string.IsNullOrEmpty(input.GeneralMessage)){
                throw new UserFriendlyException($"Por favor ingrese un mensaje.");
            }

            User userLogged = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            // Temporary solution the key must be in the appsettings
                var sendGridClient = new SendGridClient("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");
                var from = new EmailAddress("comunicadosrh@t3b.com.mx", "Soporte Tiendas 3B");
                var subject = "Contactar al administrador - Evaluación de desempeño";
                var to = new EmailAddress("desarrollo@tiendas3b.com", "Desarrollo Tiendas 3B");
                var plainTextContent = $"El empleado {userLogged.FullName} ha contactado al administrador desde la plataforma de Evaluación de desempeño. Mensaje: {input.GeneralMessage}";
                // We need create a email template
                var htmlContent = $"El empleado <strong>{userLogged.FullName}</strong> ha contactado al administrador desde la plataforma de Evaluación de desempeño. <br><br>Mensaje: <br> <strong>{input.GeneralMessage}</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

               
                await sendGridClient.SendEmailAsync(msg);
                
        }
    }
}