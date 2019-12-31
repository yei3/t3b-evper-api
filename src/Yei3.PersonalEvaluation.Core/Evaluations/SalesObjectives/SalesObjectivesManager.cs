using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Core.Evaluations.SalesObjectives
{
    public class SalesObjectivesManager : DomainService
    {
        public IAbpSession AbpSession { get; set; }
        private readonly UserManager _userManager;
        private readonly IRepository<Evaluation, long> _evaluationRepository;
        private readonly IRepository<EvaluationMeasuredQuestion, long> _evaluationMeasuredQuestionRepository;

        public SalesObjectivesManager (UserManager userManager, IRepository<Evaluation, long> evaluationRepository, IRepository<EvaluationMeasuredQuestion, long> evaluationMeasuredQuestionRepository)
        {
            _userManager = userManager;
            _evaluationRepository = evaluationRepository;
            _evaluationMeasuredQuestionRepository = evaluationMeasuredQuestionRepository;
        }

        public async Task<bool> ImportGTSalesObjectivesAsync(
            string employeeNumber,
            long autoEvaluationId,
            long evaluationId,
            decimal expectedSales,
            decimal realSales,
            decimal expectedMerma,
            decimal realMerma,
            decimal expectedMoneyDiff,
            decimal realMoneyDiff,
            decimal expectedMysteryShopper,
            decimal realMysteryShopper,
            decimal expectedRotationAV,
            decimal realRotationAV,
            decimal expectedRotationTR,
            decimal realRotationTR,
            decimal expectedCostMO,
            decimal realCostMO,
            string expectedAudits,
            string realAudits,
            decimal expectedSO,
            decimal realSO
        )
        {
            using (IUnitOfWorkCompleteHandle unitOfWork = UnitOfWorkManager.Begin())
            {
                User user = await _userManager.FindByEmployeeNumberAsync(employeeNumber);

                if (user.IsNullOrDeleted())
                {
                    
                }

                var evaSalesObjectives = await _evaluationMeasuredQuestionRepository
                    .GetAll()
                    .Include(question => question.MeasuredAnswer)
                    .Include(question => question.MeasuredQuestion)
                    .Where(question => question.EvaluationId == evaluationId)
                    .ToListAsync();

                foreach (var objective in evaSalesObjectives)
                {
                    if (objective.MeasuredQuestion.Text.Contains("Ventas"))
                    {
                        objective.Expected = expectedSales;
                        objective.MeasuredAnswer.Real = realSales;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Merma"))
                    {
                        objective.Expected = expectedMerma;
                        objective.MeasuredAnswer.Real = realMerma;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Diferencias de dinero"))
                    {
                        objective.Expected = expectedMoneyDiff;
                        objective.MeasuredAnswer.Real = realMoneyDiff;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Mystery Shopper"))
                    {
                        objective.Expected = expectedMysteryShopper;
                        objective.MeasuredAnswer.Real = realMysteryShopper;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Rotación de AV"))
                    {
                        objective.Expected = expectedRotationAV;
                        objective.MeasuredAnswer.Real = realRotationAV;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Rotación de TR"))
                    {
                        objective.Expected = expectedRotationTR;
                        objective.MeasuredAnswer.Real = realRotationTR;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Costo mano de obra"))
                    {
                        objective.Expected = expectedCostMO;
                        objective.MeasuredAnswer.Real = realCostMO;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Auditorias"))
                    {
                        objective.ExpectedText = expectedAudits;
                        objective.MeasuredAnswer.Text = realAudits;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("SO"))
                    {
                        objective.Expected = expectedSO;
                        objective.MeasuredAnswer.Real = realSO;
                    }
                }

                var autoSalesObjectives = await _evaluationMeasuredQuestionRepository
                    .GetAll()
                    .Include(question => question.MeasuredAnswer)
                    .Include(question => question.MeasuredQuestion)
                    .Where(question => question.EvaluationId == autoEvaluationId)
                    .ToListAsync();

                foreach (var objective in autoSalesObjectives)
                {
                    if (objective.MeasuredQuestion.Text.Contains("Ventas"))
                    {
                        objective.Expected = expectedSales;
                        objective.MeasuredAnswer.Real = realSales;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Merma"))
                    {
                        objective.Expected = expectedMerma;
                        objective.MeasuredAnswer.Real = realMerma;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Diferencias de dinero"))
                    {
                        objective.Expected = expectedMoneyDiff;
                        objective.MeasuredAnswer.Real = realMoneyDiff;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Mystery Shopper"))
                    {
                        objective.Expected = expectedMysteryShopper;
                        objective.MeasuredAnswer.Real = realMysteryShopper;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Rotación de AV"))
                    {
                        objective.Expected = expectedRotationAV;
                        objective.MeasuredAnswer.Real = realRotationAV;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Rotación de TR"))
                    {
                        objective.Expected = expectedRotationTR;
                        objective.MeasuredAnswer.Real = realRotationTR;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Costo mano de obra"))
                    {
                        objective.Expected = expectedCostMO;
                        objective.MeasuredAnswer.Real = realCostMO;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Auditorias"))
                    {
                        objective.ExpectedText = expectedAudits;
                        objective.MeasuredAnswer.Text = realAudits;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("SO"))
                    {
                        objective.Expected = expectedSO;
                        objective.MeasuredAnswer.Real = realSO;
                    }
                }

                await unitOfWork.CompleteAsync();
            }

            return true;
        }

        public async Task<bool> ImportGDSalesObjectivesAsync(
            string employeeNumber,
            long autoEvaluationId,
            long evaluationId,
            decimal expectedSales,
            decimal realSales,
            decimal expectedMerma,
            decimal realMerma,
            decimal expectedMOCost,
            decimal realMOCost,
            decimal expectedRotationStore,
            decimal realRotationStore,
            decimal expectedMysteryShopper,
            decimal realMysteryShopper,
            decimal expectedSO,
            decimal realSO,
            string expectedAudits,
            string realAudits,
            decimal expectedRecSche,
            decimal realRecSche,
            decimal expectedTestAV,
            decimal realTestAV
        )
        {
            using (IUnitOfWorkCompleteHandle unitOfWork = UnitOfWorkManager.Begin())
            {
                User user = await _userManager.FindByEmployeeNumberAsync(employeeNumber);

                if (user.IsNullOrDeleted())
                {
                    
                }

                var evaSalesObjectives = await _evaluationMeasuredQuestionRepository
                    .GetAll()
                    .Include(question => question.MeasuredAnswer)
                    .Include(question => question.MeasuredQuestion)
                    .Where(question => question.EvaluationId == evaluationId)
                    .ToListAsync();

                foreach (var objective in evaSalesObjectives)
                {
                    if (objective.MeasuredQuestion.Text.Contains("Ventas"))
                    {
                        objective.Expected = expectedSales;
                        objective.MeasuredAnswer.Real = realSales;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Merma"))
                    {
                        objective.Expected = expectedMerma;
                        objective.MeasuredAnswer.Real = realMerma;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Costo mano de obra"))
                    {
                        objective.Expected = expectedMOCost;
                        objective.MeasuredAnswer.Real = realMOCost;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Rotación Tienda"))
                    {
                        objective.Expected = expectedRotationStore;
                        objective.MeasuredAnswer.Real = realRotationStore;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Mystery Shopper"))
                    {
                        objective.Expected = expectedMysteryShopper;
                        objective.MeasuredAnswer.Real = realMysteryShopper;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("SO promedio por Tienda"))
                    {
                        objective.Expected = expectedSO;
                        objective.MeasuredAnswer.Real = realSO;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Auditorias"))
                    {
                        objective.ExpectedText = expectedAudits;
                        objective.MeasuredAnswer.Text = realAudits;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Entrega de recibos y horarios"))
                    {
                        objective.Expected = expectedRecSche;
                        objective.MeasuredAnswer.Real = realRecSche;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Aprobación de exámenes AV"))
                    {
                        objective.Expected = expectedTestAV;
                        objective.MeasuredAnswer.Real = realTestAV;
                    }
                }

                var autoSalesObjectives = await _evaluationMeasuredQuestionRepository
                    .GetAll()
                    .Include(question => question.MeasuredAnswer)
                    .Include(question => question.MeasuredQuestion)
                    .Where(question => question.EvaluationId == autoEvaluationId)
                    .ToListAsync();

                foreach (var objective in autoSalesObjectives)
                {
                    if (objective.MeasuredQuestion.Text.Contains("Ventas"))
                    {
                        objective.Expected = expectedSales;
                        objective.MeasuredAnswer.Real = realSales;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Merma"))
                    {
                        objective.Expected = expectedMerma;
                        objective.MeasuredAnswer.Real = realMerma;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Costo mano de obra"))
                    {
                        objective.Expected = expectedMOCost;
                        objective.MeasuredAnswer.Real = realMOCost;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Rotación Tienda"))
                    {
                        objective.Expected = expectedRotationStore;
                        objective.MeasuredAnswer.Real = realRotationStore;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Mystery Shopper"))
                    {
                        objective.Expected = expectedMysteryShopper;
                        objective.MeasuredAnswer.Real = realMysteryShopper;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("SO promedio por Tienda"))
                    {
                        objective.Expected = expectedSO;
                        objective.MeasuredAnswer.Real = realSO;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Auditorias"))
                    {
                        objective.ExpectedText = expectedAudits;
                        objective.MeasuredAnswer.Text = realAudits;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Entrega de recibos y horarios"))
                    {
                        objective.Expected = expectedRecSche;
                        objective.MeasuredAnswer.Real = realRecSche;
                    }
                    if (objective.MeasuredQuestion.Text.Contains("Aprobación de exámenes AV"))
                    {
                        objective.Expected = expectedTestAV;
                        objective.MeasuredAnswer.Real = realTestAV;
                    }
                }

                await unitOfWork.CompleteAsync();
            }

            return true;
        }
    }
}
