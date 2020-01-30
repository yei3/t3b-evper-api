using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Runtime.Session;
using Yei3.PersonalEvaluation.Authorization.Users;
using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Yei3.PersonalEvaluation.Evaluations;
using System;
using Abp.Net.Mail;
using System.Net.Mail;

namespace Yei3.PersonalEvaluation.Notifications
{
    public class NotificationsAppService : ApplicationService, INotificationsAppService
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IUserNotificationManager _userNotificationManager;

        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly UserManager UserManager;
        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitRepository;
        private readonly IEmailSender _emailSender;

        public NotificationsAppService(INotificationPublisher notiticationPublisher, IRepository<Evaluation, long> evaluationRepository, IUserNotificationManager userNotificationManager, INotificationSubscriptionManager notificationSubscriptionManager, UserManager userManager, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitRepository, IEmailSender emailSender)
        {
            _notificationPublisher = notiticationPublisher;
            EvaluationRepository = evaluationRepository;
            _userNotificationManager = userNotificationManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            UserManager = userManager;
            OrganizationUnitRepository = organizationUnitRepository;
            _emailSender = emailSender;
        }

        public async Task<List<UserNotification>> getAll()
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            List<UserNotification> lista = await _userNotificationManager.GetUserNotificationsAsync(new Abp.UserIdentifier(administratorUser.TenantId, administratorUser.Id), 0, 0, 100);
            List<UserNotification> resultados = new List<UserNotification>();
            foreach (UserNotification userNotif in lista)
            {
                if (userNotif.Notification.CreationTime > DateTime.Now.AddDays(-5))
                {
                    resultados.Add(userNotif);

                }
            }
            return resultados;
        }

        public async Task<int> getNotifCount()
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            return await _userNotificationManager.GetUserNotificationCountAsync(new Abp.UserIdentifier(administratorUser.TenantId, administratorUser.Id), 0);
        }

        public async Task Publish_SentGeneralUserNotification(string senderUserName, string generalMessage)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            UserIdentifier targetUserId = new UserIdentifier(administratorUser.TenantId, administratorUser.Id);
            await _notificationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData(senderUserName, generalMessage), userIds: new[] { targetUserId });
        }

        public async Task Publish_SentGeneralMultipleUserNotification(CreateNotificationDto input)
        {
            if (string.IsNullOrEmpty(input.GeneralMessage))
            {
                throw new UserFriendlyException($"Por favor ingrese un mensaje.");
            }
            if (input.JobDescriptions.IsNullOrEmpty<string>() && input.UserIds.IsNullOrEmpty<long>() && input.OrganizationUnitIds.IsNullOrEmpty<long>())
            {
                throw new UserFriendlyException($"Debe seleccionar algún destinatario para el mensaje.");
            }
            List<User> users = await UserManager
                .Users
                .Where(user => input.UserIds.Contains(user.Id))
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

            if (!input.JobDescriptions.IsNullOrEmpty<string>())
            {
                List<User> usersJob = await UserManager
                .Users
                .Where(user => input.JobDescriptions.Contains(user.JobDescription))
                .ToListAsync();

                users.AddRange(
                    usersJob
                );

            }

            await _notificationPublisher.PublishAsync("GeneralNotification",
                        new SentGeneralUserNotificationData(input.SenderName, input.GeneralMessage),
                        userIds: users.Select(user => new UserIdentifier(user.TenantId, user.Id)).ToArray());
        }

        public async Task Publish_SentBossGeneralUserNotification(long objectiveId)
        {

            User userLogged = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            if (!string.IsNullOrEmpty(userLogged.ImmediateSupervisor))
            {
                List<User> users = await UserManager
                .Users
                .Where(user => userLogged.ImmediateSupervisor.Equals(user.JobDescription))
                .ToListAsync();
                if (!users.IsNullOrEmpty<User>())
                {
                    User bossUser = users[0];
                    UserIdentifier targetUserId = new UserIdentifier(bossUser.TenantId, bossUser.Id);
                    await _notificationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData("Administrador", "El evaluado " + userLogged.FullName + " ha completado un objetivo."), userIds: new[] { targetUserId });
                }

            }


        }

        public async Task Publish_SentReviewNotification(SentReviewNotificationData input)
        {
            User supervisor = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            Evaluation evaluation = EvaluationRepository.FirstOrDefault(input.EvaluationId);
            User collaborator = await UserManager.GetUserByIdAsync(evaluation.UserId);
            UserIdentifier targetUserId = new UserIdentifier(supervisor.TenantId, collaborator.Id);

            await _notificationPublisher.PublishAsync(
                "GeneralNotification",
                new SentGeneralUserNotificationData(supervisor.FullName, "Se agendó la revisión de tu evaluación en la fecha " + input.DateReview),
                userIds: new[] { targetUserId }
            );

            MailMessage mail = new MailMessage(
                new MailAddress("comunicadosrh@t3b.com.mx", "Soporte Tiendas 3B"),
                new MailAddress(collaborator.EmailAddress, collaborator.FullName)
            );

            mail.Subject = "Revisión de evaluación - Evaluación de desempeño";
            mail.Body = $"Se agendó la revisión de tu evaluación en la siguiente fecha: {input.DateReview}. <br/> Hola {collaborator.Name} <br/>Se agendó la revisión de tu evaluación en la siguiente fecha: <strong>{input.DateReview}</strong>";
            mail.IsBodyHtml = true;

            await _emailSender.SendAsync(mail);
        }

        public async Task Publish_SendBossCloseEvaluationNotification()
        {
            User userLogged = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            if (!string.IsNullOrEmpty(userLogged.ImmediateSupervisor))
            {
                List<User> users = await UserManager
                .Users
                .Where(user => userLogged.ImmediateSupervisor.Equals(user.JobDescription))
                .ToListAsync();
                if (!users.IsNullOrEmpty<User>())
                {
                    User bossUser = users[0];
                    UserIdentifier targetUserId = new UserIdentifier(bossUser.TenantId, bossUser.Id);
                    await _notificationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData("Cierre de evaluación.", "El evaluado " + userLogged.FullName + " ha concluido el proceso de evaluación."), userIds: new[] { targetUserId });
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
            await _notificationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData("Cierre de evaluación", "Se validó el cierre de tu evaluación."), userIds: new[] { targetUserId });
        }

        public async Task Publish_SentGeneralMessageToAdminMail(CreateNotificationDto input)
        {
            if (string.IsNullOrEmpty(input.GeneralMessage))
            {
                throw new UserFriendlyException($"Por favor ingrese un mensaje.");
            }

            User userLogged = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            MailMessage mail = new MailMessage(
                new MailAddress("comunicadosrh@t3b.com.mx", "Soporte Tiendas 3B"),
                new MailAddress("desarrollo@tiendas3b.com", "Desarrollo Tiendas 3B")
            );

            mail.Subject = "Contactar al administrador - Evaluación de desempeño";
            mail.Body = $"El empleado {userLogged.FullName} ha contactado al administrador desde la plataforma de Evaluación de desempeño. Mensaje: {input.GeneralMessage} <br/> El empleado <strong>{userLogged.FullName}</strong> ha contactado al administrador desde la plataforma de Evaluación de desempeño. <br><br>Mensaje: <br> <strong>{input.GeneralMessage}</strong>";
            mail.IsBodyHtml = true;

            await _emailSender.SendAsync(mail);

        }

        public async Task Publish_SentReopenedUserNotification(SentReopenedNotificationData dto)
        {

            User user = await UserManager.GetUserByIdAsync(dto.UserId);
            UserIdentifier targetUserId = new UserIdentifier(user.TenantId, user.Id);
            await _notificationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData("Atención", "Se ha reabierto el siguiente objetivo: '" + dto.ObjectiveName + "'"), userIds: new[] { targetUserId });

        }
    }
}