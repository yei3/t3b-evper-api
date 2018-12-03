using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Questions;

namespace Yei3.PersonalEvaluation.Evaluations
{

    public class EvaluationAppService : ApplicationService, IEvaluationAppService
    {
        private readonly IEvaluationManager EvaluationManager;
        private readonly IRepository<EvaluationTemplates.EvaluationTemplate, long> EvaluationTemplateRepository;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitRepository;
        private readonly UserManager UserManager;

        public EvaluationAppService(IRepository<EvaluationTemplates.EvaluationTemplate, long> evaluationTemplateRepository, IRepository<Evaluation, long> evaluationRepository, UserManager userManager, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitRepository)
        {
            EvaluationTemplateRepository = evaluationTemplateRepository;
            EvaluationRepository = evaluationRepository;
            UserManager = userManager;
            OrganizationUnitRepository = organizationUnitRepository;
        }

        public async Task ApplyEvaluationTemplate(CreateEvaluationDto input)
        {
            EvaluationTemplates.EvaluationTemplate evaluationTemplate = await
                EvaluationTemplateRepository
                        .GetAll()
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.UnmeasuredQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.MeasuredQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.ChildSections)
                        .ThenInclude(section => section.UnmeasuredQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.ChildSections)
                        .ThenInclude(section => section.MeasuredQuestions)
                    .FirstOrDefaultAsync(currentEvaluationTemplate => currentEvaluationTemplate.Id == input.EvaluationTemplateId);

            if (evaluationTemplate.IsNullOrDeleted())
            {
                throw new UserFriendlyException($"Formato de Evaluacion {input.EvaluationTemplateId} no encontrado.");
            }

            List<User> users = new List<User>();

            foreach (long inputOrganizationUnitId in input.OrganizationUnitIds)
            {
                Abp.Organizations.OrganizationUnit currentOrganizationUnit = await 
                    OrganizationUnitRepository.FirstOrDefaultAsync(inputOrganizationUnitId);
                users.AddRange(await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, false));
            }

            foreach (User user in users.Distinct())
            {
                Abp.Organizations.OrganizationUnit userOrganizationUnit = (await UserManager.GetOrganizationUnitsAsync(user))
                    .First();

                User supervisor = (await UserManager.GetUsersInOrganizationUnit(userOrganizationUnit, false))
                    .FirstOrDefault(currentUser =>
                        UserManager.IsInRoleAsync(currentUser, StaticRoleNames.Tenants.Supervisor).GetAwaiter()
                            .GetResult());

                long? supervisorId = null;

                if (!supervisor.IsNullOrDeleted())
                {
                    supervisorId = supervisor.Id;
                }

                Evaluation currentEvaluation = new Evaluation(
                    input.Name,
                    evaluationTemplate.Id,
                    user.Id,
                    input.StartDate,
                    input.EndDate);

                currentEvaluation.SetRevision(
                    currentEvaluation.EvaluationId,
                    supervisorId.GetValueOrDefault(3),
                    input.EndDate);

                long evaluationId = await EvaluationRepository.InsertAndGetIdAsync(currentEvaluation);

                currentEvaluation = await EvaluationRepository.FirstOrDefaultAsync(evaluationId);



                List<MeasuredQuestion> measuredQuestions = new List<MeasuredQuestion>();
                List<UnmeasuredQuestion> unmeasuredQuestions = new List<UnmeasuredQuestion>();

                foreach (Sections.Section evaluationTemplateSection in evaluationTemplate.Sections)
                {
                    measuredQuestions.AddRange(evaluationTemplateSection.MeasuredQuestions);
                    unmeasuredQuestions.AddRange(evaluationTemplateSection.UnmeasuredQuestions);
                }

                foreach (UnmeasuredQuestion unmeasuredQuestion in unmeasuredQuestions)
                {
                    currentEvaluation.Questions.Add(new EvaluationQuestion(
                        currentEvaluation.EvaluationId,
                        unmeasuredQuestion.Id,
                        currentEvaluation.EndDateTime));
                }

                foreach (MeasuredQuestion measuredQuestion in measuredQuestions)
                {
                    currentEvaluation.Questions.Add(new EvaluationQuestion(
                        currentEvaluation.EvaluationId,
                        measuredQuestion.Id,
                        currentEvaluation.EndDateTime));
                }

                foreach (EvaluationQuestion currentEvaluationQuestion in currentEvaluation.Questions)
                {
                    currentEvaluationQuestion.SetAnswer(currentEvaluationQuestion.QuestionId, evaluationId);
                }


                await CurrentUnitOfWork.SaveChangesAsync();
            }


        }

        public async Task<ICollection<Evaluation>> GetAllAsync()
        {
            return await EvaluationRepository.GetAllListAsync();
        }
    }
}