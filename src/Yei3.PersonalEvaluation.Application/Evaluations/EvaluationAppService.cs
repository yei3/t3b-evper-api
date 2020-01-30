using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Binnacle;
using Yei3.PersonalEvaluation.Evaluations.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Abp.Runtime.Caching;
using Yei3.PersonalEvaluation.Net.MimeTypes;
using OfficeOpenXml;
using Abp.Timing;

namespace Yei3.PersonalEvaluation.Evaluations
{

    public class EvaluationAppService : ApplicationService, IEvaluationAppService
    {
        private readonly IRepository<EvaluationTemplates.EvaluationTemplate, long> EvaluationTemplateRepository;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitRepository;
        private readonly UserManager UserManager;
        private readonly IRepository<EvaluationQuestions.NotEvaluableQuestion, long> NotEvaluableQuestionRepository;
        private readonly ICacheManager CacheManager;

        private IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public EvaluationAppService(IRepository<EvaluationTemplates.EvaluationTemplate, long> evaluationTemplateRepository, IRepository<Evaluation, long> evaluationRepository, UserManager userManager, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitRepository, IRepository<EvaluationQuestions.NotEvaluableQuestion, long> notEvaluableQuestionRepository, ICacheManager cacheManager)
        {
            EvaluationTemplateRepository = evaluationTemplateRepository;
            EvaluationRepository = evaluationRepository;
            UserManager = userManager;
            OrganizationUnitRepository = organizationUnitRepository;
            NotEvaluableQuestionRepository = notEvaluableQuestionRepository;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            CacheManager = cacheManager;
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

            if (input.OrganizationUnitIds.IsNullOrEmpty())
            {
                input.OrganizationUnitIds = new List<long>() { OrganizationUnitRepository.Single(organizationUnit => organizationUnit.Code.Equals("00001")).Id };
            }

            foreach (long inputOrganizationUnitId in input.OrganizationUnitIds)
            {
                Abp.Organizations.OrganizationUnit currentOrganizationUnit = await
                    OrganizationUnitRepository.FirstOrDefaultAsync(inputOrganizationUnitId);

                List<User> allUsers = await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true);

                users.AddRange(
                    allUsers
                        .Where(user => UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Collaborator).GetAwaiter().GetResult()
                        )
                        .WhereIf(!input.JobDescriptions.IsNullOrEmpty(), user => input.JobDescriptions.Contains(user.JobDescription))
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
                    UserManager.Users.FirstOrDefault(currentUser => currentUser.JobDescription == user.ImmediateSupervisor);

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
                        EvaluationQuestionStatus.Unanswered,
                        measuredQuestion.Expected,
                        measuredQuestion.ExpectedText);

                    evaluationMeasuredQuestion.SetAnswer(measuredQuestion.Id);

                    currentEvaluation.Questions.Add(evaluationMeasuredQuestion);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                var lastEvaluation = EvaluationRepository
                    .GetAll()
                    .Include(evaluation => evaluation.Questions)
                    .Include(evaluation => evaluation.Template)
                    .ThenInclude(template => template.Sections)
                    .Include(evaluation => evaluation.Template)
                    .ThenInclude(template => template.Sections)
                    .ThenInclude(section => section.ChildSections)
                    .Include(evaluation => evaluation.Template)
                    .ThenInclude(template => template.Sections)
                    .ThenInclude(section => section.ChildSections)
                    .ThenInclude(section => section.ParentSection)
                    //.Where(evaluation => evaluation.EvaluationId == evaluationTemplate.Id)
                    .Where(evaluation => evaluation.UserId == user.Id)
                    .Where(evaluation => evaluation.Id != evaluationId)
                    .Where(evaluation => evaluation.Template.IsAutoEvaluation)
                    .OrderByDescending(evaluation => evaluation.CreationTime)
                    //.WhereIf(currentEvaluation.Template.IsAutoEvaluation,
                    //    evaluation => evaluation.Template.IsAutoEvaluation)
                    //.Skip(currentEvaluation.Template.IsAutoEvaluation ? 0 : 1)
                    .FirstOrDefault();

                if (lastEvaluation.IsNullOrDeleted()) continue;

                string objectivesToLoad = currentEvaluation.Template.IsAutoEvaluation
                    ? AppConsts.SectionNextObjectivesName
                    : AppConsts.SectionObjectivesName;

                long? lastEvaluationNextObjectiveSectionId = lastEvaluation.Template.Sections
                    .Where(section => section.ParentId.HasValue)
                    .Where(section => section.ParentSection.Name.StartsWith(objectivesToLoad, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefault(section => section.Name == AppConsts.SectionObjectivesName)?.Id;

                // No utilizada la variable currentEvaluation porque no tiene los valores de la propiedad de navegacion ParentSection por ser muy profunda
                // en cambio utilizamos el repositorio para que vaya a la base de datos a buscar el id del Parent Section

                long? currentEvaluationObjectivesSectionId = EvaluationRepository
                    .GetAll()
                    .Include(evaluation => evaluation.Template)
                    .ThenInclude(template => template.Sections)
                    .Include(evaluation => evaluation.Template)
                    .ThenInclude(template => template.Sections)
                    .ThenInclude(section => section.ChildSections)
                    .Include(evaluation => evaluation.Template)
                    .ThenInclude(template => template.Sections)
                    .ThenInclude(section => section.ChildSections)
                    .ThenInclude(section => section.ParentSection)
                    .Where(evaluation => evaluation.Id == currentEvaluation.Id)
                    .Select(evaluation => evaluation.Template)
                    .Select(template =>
                        template.Sections
                            // Hasta aqui es exactamente el valor que esta en currentEvaluation, utilizamos el repo por lo explicado anteriormente
                            .Where(section => section.ParentId.HasValue)
                            .Where(section => section.ParentSection.Name.StartsWith(AppConsts.SectionObjectivesName, StringComparison.CurrentCultureIgnoreCase))
                            .FirstOrDefault(section => section.Name == AppConsts.SectionObjectivesName)
                        ).FirstOrDefault()?.Id;




                if (!lastEvaluationNextObjectiveSectionId.HasValue || !currentEvaluationObjectivesSectionId.HasValue) continue;

                IQueryable<EvaluationQuestions.NotEvaluableQuestion> notEvaluableQuestions = NotEvaluableQuestionRepository
                    .GetAll()
                    .Include(question => question.NotEvaluableAnswer)
                    .Include(question => question.Binnacle)
                    .Where(question => question.SectionId == lastEvaluationNextObjectiveSectionId.Value)
                    .Where(question => question.EvaluationId == lastEvaluation.Id);

                foreach (EvaluationQuestions.NotEvaluableQuestion notEvaluableQuestion in notEvaluableQuestions)
                {

                    EvaluationQuestions.NotEvaluableQuestion currentQuestion = new EvaluationQuestions.NotEvaluableQuestion(
                        currentEvaluationObjectivesSectionId.Value,
                        notEvaluableQuestion.Text,
                        currentEvaluation.Id,
                        notEvaluableQuestion.NotEvaluableAnswer.CommitmentTime,
                        notEvaluableQuestion.Status)
                    {
                        SectionId = currentEvaluationObjectivesSectionId.Value
                    };

                    currentQuestion.SetAnswer(
                        currentEvaluation.Id,
                        notEvaluableQuestion.NotEvaluableAnswer.Text,
                        notEvaluableQuestion.NotEvaluableAnswer.CommitmentTime
                    );

                    await NotEvaluableQuestionRepository.InsertAsync(currentQuestion);
                    foreach (ObjectiveBinnacle objectiveBinnacle in notEvaluableQuestion.Binnacle)
                    {
                        ObjectiveBinnacle currentObjectiveBinnacle = new ObjectiveBinnacle(
                            objectiveBinnacle.Text,
                            currentQuestion.Id,
                            objectiveBinnacle.CreatorUserId
                        );

                        currentQuestion.Binnacle.Add(currentObjectiveBinnacle);
                    }
                }
            }
        }

        public Task<ICollection<EvaluationDto>> GetAll()
        {
            IIncludableQueryable<Evaluation, ICollection<UnmeasuredQuestion>> resultEvaluations = EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.User)
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

        public async Task<EvaluationDto> Get(long id)
        {
            Evaluation resultEvaluation = await EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.User)
                .Include(evaluation => evaluation.Revision)
                .ThenInclude(revision => revision.ReviewerUser)
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationMeasuredQuestion)evaluationQuestion).MeasuredAnswer)
                .ThenInclude(answer => answer.EvaluationMeasuredQuestion)
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

            var evaluationDto = resultEvaluation.MapTo<EvaluationDto>();

            evaluationDto.Template.PurgeSubSections();

            User evaluatorUser = resultEvaluation.Revision.ReviewerUser;

            evaluationDto.EvaluatorFullName = evaluatorUser.FullName;

            return evaluationDto;
        }

        //TODO: Enpoint to return the period of a user by his last evaluations
        public async Task<string> GetUserPeriod()
        {
            var userId = AbpSession.GetUserId();

            var lastEvaluation = await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .OrderByDescending(evaluation => evaluation.CreationTime)
                .Select(evaluation => evaluation.CreationTime)
                .FirstOrDefaultAsync();

            if (lastEvaluation != null)
            {

            }

            return "";
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
                        : firstEvaluation.EndDateTime.AddMonths(1) <= DateTime.Now
                            ? EvaluationStatus.Finished
                            : EvaluationStatus.Pending,
                });
            }

            return await Task.FromResult(result);
        }

        public async Task FinalizeEvaluation(EntityDto<long> input)
        {
            Evaluation evaluation = await EvaluationRepository.FirstOrDefaultAsync(input.Id);

            if (evaluation.IsNullOrDeleted())
            {
                throw new EntityNotFoundException(typeof(Evaluation), evaluation.Id);
            }

            evaluation.FinishEvaluation();
            evaluation.Activate();
        }

        public async Task Delete(long id)
        {
            Evaluation evaluation = EvaluationRepository.FirstOrDefault(id);

            if (evaluation.IsNullOrDeleted())
            {
                return;
            }

            await EvaluationRepository.DeleteAsync(evaluation);
        }

        public Task ClosingComment(EvaluationCloseDto evaluationClose)
        {
            Evaluation evaluation = EvaluationRepository.FirstOrDefault(evaluationClose.Id);

            Evaluation autoEvaluation = EvaluationRepository
                .GetAll()
                .Where(evaluations => evaluations.Term == evaluation.Term)
                .Where(evaluations => evaluations.UserId == evaluation.UserId)
                .OrderByDescending(evaluations => evaluations.CreationTime)
                .FirstOrDefault(evaluations => evaluations.Id != evaluationClose.Id);

            if (!evaluation.IsNullOrDeleted() && !autoEvaluation.IsNullOrDeleted())
            {
                evaluation.ClosingComment = evaluationClose.Comment;
                evaluation.IsActive = false;
                autoEvaluation.IsActive = false;
            }
            return Task.CompletedTask;
        }

        public async Task ReopenEvaluation(EntityDto<long> input)
        {
            Evaluation evaluation = await EvaluationRepository.FirstOrDefaultAsync(input.Id);

            if (evaluation.IsNullOrDeleted())
            {
                throw new EntityNotFoundException(typeof(Evaluation), evaluation.Id);
            }

            evaluation.UnfinishEvaluation();
            evaluation.Activate();
        }

        protected IQueryable<EvaluationStatusListItemDto> GetEvaluationStatusAsQueryable(EvaluationStatusInputDto input)
        {
            return EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.User)
                .Include(evaluation => evaluation.Template)
                .WhereIf(input.StartDateTime.HasValue, evaluation => evaluation.CreationTime >= input.StartDateTime.Value)
                .WhereIf(input.EndDateTime.HasValue, evaluation => evaluation.CreationTime <= input.EndDateTime.Value)
                .Select(evaluation => new EvaluationStatusListItemDto
                {
                    Id = evaluation.Id,
                    EmployeeNumber = evaluation.User.EmployeeNumber,
                    EmployeeName = evaluation.User.Name,
                    EmployeeSurname = evaluation.User.Surname,
                    Area = evaluation.User.Area,
                    Region = evaluation.User.Region,
                    EvaluationName = evaluation.Name,
                    IsAutoEvaluation = evaluation.Template.IsAutoEvaluation,
                    IncludePastObjectives = evaluation.Template.IncludePastObjectives,
                    Status = evaluation.Status
                })
                .OrderBy(evaluationStatus => evaluationStatus.Id)
                .ThenBy(evaluationStatus => evaluationStatus.EmployeeName);
        }

        public async Task<PagedResultDto<EvaluationStatusListItemDto>> GetEvaluationsStatus(EvaluationStatusInputDto input)
        {

            IQueryable<EvaluationStatusListItemDto> evaluationStatuses = GetEvaluationStatusAsQueryable(input);

            int count = 0;

            if (input.ApplyPagination)
            {
                count = await AsyncQueryableExecuter.CountAsync(evaluationStatuses);

                evaluationStatuses = evaluationStatuses
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount);
            }

            return new PagedResultDto<EvaluationStatusListItemDto>(
                count,
                await evaluationStatuses.ToListAsync()
            );
        }

        public FileDto GetEvaluationsStatusSheet(EvaluationStatusInputDto input)
        {
            IQueryable<EvaluationStatusListItemDto> evaluationStatuses = GetEvaluationStatusAsQueryable(input);
            FileDto file = CreateExcelPackage($"EstatusEvaluaciones_{Clock.Now:yyyyMMdd_HH:mm}.xlsx", excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("EstatusEvaluaciones");
                    sheet.OutLineApplyStyle = true;

                    string[] headerTexts = new string[] {
                        "COLABORADOR",
                        "NOMBRE",
                        "APELLIDOS",
                        "REGION",
                        "AREA",
                        "NOMBRE DE EVALUACION",
                        "TIPO",
                        "INCLUYE OBJETIVOS ANTERIORES",
                        "EVALUACION",
                        "ESTATUS"
                    };

                    for (int i = 0; i < headerTexts.Length; i++)
                    {
                        string headerText = headerTexts[i];

                        sheet.Cells[1, i + 1].Value = headerText;
                        sheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    AddExcelObjects(sheet, 2, evaluationStatuses.ToList(),
                        _ => _.EmployeeNumber,
                        _ => _.EmployeeName,
                        _ => _.EmployeeSurname,
                        _ => _.Region,
                        _ => _.Area,
                        _ => _.EvaluationName,
                        _ => _.IsAutoEvaluation ? "AED" : "ED",
                        _ => _.IncludePastObjectives ? "Incluye Objetivos Anteriores" : "Sin Objetivos Anteriores",
                        _ => _.Id,
                        _ => _.Status == EvaluationStatus.NonInitiated ? "No Iniciada"
                            : _.Status == EvaluationStatus.Finished ? "Finalizada"
                            : _.Status == EvaluationStatus.PendingReview ? "Pte. Revision"
                            : _.Status == EvaluationStatus.Validated ? "Cerrada"
                            : "No Iniciada"
                    );
                });

            return file;
        }

        // TODO abstraer todo esto en un XLSX manager que genere excels de cualquier colleccion
        // Start Excel Stuff
        protected void AddExcelObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }
        
        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                SaveFileToCache(excelPackage, file);
            }

            return file;
        }

        protected void SaveFileToCache(ExcelPackage excelPackage, FileDto file)
        {
            CacheManager.GetCache(AppConsts.TempEvaluationStatusesFileName).Set(file.FileToken, excelPackage.GetAsByteArray(), new TimeSpan(0, 0, 1, 0)); // expire time is 1 min by default
        }
        // End Excel Stuff
    }
}