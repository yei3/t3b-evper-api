using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (!await UserManager.IsInRoleAsync(administratorUser, StaticRoleNames.Tenants.Administrator))
            {
                throw new UserFriendlyException($"Usuario {administratorUser.FullName} no es un Administrador.");
            }

            EvaluationTemplates.EvaluationTemplate evaluationTemplate = await
                EvaluationTemplateRepository
                        .GetAll()
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.UnmeasuredQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.MeasuredQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.NotEvaluableQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.ChildSections)
                        .ThenInclude(section => section.UnmeasuredQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.ChildSections)
                        .ThenInclude(section => section.MeasuredQuestions)
                        .Include(currentEvaluationTemplate => currentEvaluationTemplate.Sections)
                        .ThenInclude(section => section.ChildSections)
                        .ThenInclude(section => section.NotEvaluableQuestions)
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

                List<User> allUsers = await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true);

                users.AddRange(
                    allUsers
                        .Where(user => UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Collaborator).GetAwaiter().GetResult()
                        )
                        .Where(user => input.JobDescriptions.Contains(user.JobDescription))
                    );
            }

            foreach (User user in users.Distinct())
            {
                Evaluation currentEvaluation = new Evaluation(
                    input.Name,
                    evaluationTemplate.Id,
                    user.Id,
                    input.StartDate,
                    input.EndDate);

                User immediateSupervisor =
                    users.FirstOrDefault(currentUser => currentUser.JobDescription == user.ImmediateSupervisor);

                currentEvaluation.SetRevision(
                    currentEvaluation.EvaluationId,
                    immediateSupervisor == null ? user.Id : immediateSupervisor.Id,
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
                    EvaluationUnmeasuredQuestion evaluationUnmeasuredQuestion = new EvaluationUnmeasuredQuestion(
                        currentEvaluation.EvaluationId,
                        unmeasuredQuestion.Id,
                        currentEvaluation.EndDateTime,
                        EvaluationQuestionStatus.Unanswered);

                    evaluationUnmeasuredQuestion.SetAnswer(unmeasuredQuestion.Id);

                    currentEvaluation.Questions.Add(evaluationUnmeasuredQuestion);
                }

                foreach (MeasuredQuestion measuredQuestion in measuredQuestions)
                {
                    EvaluationMeasuredQuestion evaluationMeasuredQuestion = new EvaluationMeasuredQuestion(
                        currentEvaluation.EvaluationId,
                        measuredQuestion.Id,
                        currentEvaluation.EndDateTime,
                        EvaluationQuestionStatus.Unanswered);

                    evaluationMeasuredQuestion.SetAnswer(measuredQuestion.Id);

                    currentEvaluation.Questions.Add(evaluationMeasuredQuestion);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        public Task<ICollection<EvaluationDto>> GetAll()
        {
            IIncludableQueryable<Evaluation, ICollection<UnmeasuredQuestion>> resultEvaluations = EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationMeasuredQuestion)evaluationQuestion).MeasuredAnswer)
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationUnmeasuredQuestion)evaluationQuestion).UnmeasuredAnswer)
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationQuestions.NotEvaluableQuestion)evaluationQuestion).NotEvaluableAnswer)
                .Include(evaluation => evaluation.Template)
                .ThenInclude(evaluationTemplate => evaluationTemplate.Sections)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.ChildSections)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.MeasuredQuestions)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.NotEvaluableQuestions)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.UnmeasuredQuestions);

            ICollection<EvaluationDto> evaluationsDto = resultEvaluations.MapTo<ICollection<EvaluationDto>>();

            foreach (EvaluationDto evaluationDto in evaluationsDto)
            {
                evaluationDto.Template.PurgeSubSections();
            }

            return Task.FromResult(evaluationsDto);
        }

        public async Task Delete(long id)
        {
            Evaluation evaluation = EvaluationRepository.FirstOrDefault(id);

            if (evaluation.IsNullOrDeleted())
            {
                return;
            }

            if (DateTime.Now.IsBetween(evaluation.StartDateTime, evaluation.EndDateTime))
            {
                throw new UserFriendlyException("La evaluación está activa, por el momento no se puede eliminar.");
            }

            await EvaluationRepository.DeleteAsync(evaluation);
        }

        public async Task<EvaluationDto> Get(long id)
        {
            Evaluation resultEvaluation = await EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationMeasuredQuestion)evaluationQuestion).MeasuredAnswer)
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationUnmeasuredQuestion)evaluationQuestion).UnmeasuredAnswer)
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationQuestions.NotEvaluableQuestion)evaluationQuestion).NotEvaluableAnswer)
                .Include(evaluation => evaluation.Template)
                .ThenInclude(evaluationTemplate => evaluationTemplate.Sections)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.ChildSections)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.MeasuredQuestions)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .Include(evaluation => evaluation.Template.Sections)
                .ThenInclude(section => section.NotEvaluableQuestions)
                .FirstOrDefaultAsync(evaluation => evaluation.Id == id);

            return resultEvaluation.MapTo<EvaluationDto>();
        }

        public async Task<ICollection<AdministratorEvaluationSummaryDto>> GetAdministratorEvaluationSummary()
        {
            var evaluations = EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.Template)
                .GroupBy(evaluation => new
                {
                    EvaluationTemplateId = evaluation.EvaluationId,
                    CreationTime = evaluation.CreationTime.DayOfYear,
                    StartTime = evaluation.StartDateTime,
                    EndTime = evaluation.EndDateTime,
                    CreatorUserId = evaluation.CreatorUserId,
                    Term = evaluation.Term
                });

            List<AdministratorEvaluationSummaryDto> result = new List<AdministratorEvaluationSummaryDto>();

            foreach (var evaluation in evaluations)
            {
                var firstEvaluation = evaluation.First();
                result.Add(new AdministratorEvaluationSummaryDto
                {
                    Description = firstEvaluation.Template.Description,
                    Name = firstEvaluation.Name,
                    EndDateTime = firstEvaluation.EndDateTime,
                    EvaluationTemplateId = firstEvaluation.EvaluationId,
                    Status = firstEvaluation.StartDateTime > DateTime.Now
                        ? EvaluationStatus.NonInitiated
                        : firstEvaluation.EndDateTime <= DateTime.Now
                            ? EvaluationStatus.Finished
                            : EvaluationStatus.Pending,
                });
            }

            return await Task.FromResult(result);
        }
    }
}