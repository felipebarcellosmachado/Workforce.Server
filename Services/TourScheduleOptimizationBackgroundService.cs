using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Dto;
using Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity;
using Workforce.Realization.Application.Core.TourScheduleManagement.Service;
using Workforce.Realization.Infrastructure.External.Db;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveTake;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourSchedule.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourScheduleOptimization;

namespace Workforce.Server.Services
{
    /// <summary>
    /// Serviço responsável por processar otimizações de Tour Schedule em background usando Hangfire.
    /// </summary>
    public class TourScheduleOptimizationBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TourScheduleOptimizationBackgroundService> _logger;

        public TourScheduleOptimizationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<TourScheduleOptimizationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Executa a otimização de Tour Schedule em background.
        /// Este método é invocado pelo Hangfire.
        /// </summary>
        /// <param name="parameters">Parâmetros de otimização</param>
        public async Task ProcessOptimizationAsync(TourScheduleOptimizationParameters parameters)
        {
            _logger.LogInformation(
                "Iniciando processamento de otimização em background. OptimizationId: {OptimizationId}",
                parameters.TourScheduleOptimizationId);

            // Criar um scope para resolver dependências scoped
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
            var repository = scope.ServiceProvider.GetRequiredService<TourScheduleOptimizationRepository>();
            var tourScheduleRepository = scope.ServiceProvider.GetRequiredService<TourScheduleRepository>();
            var leaveTakeRepository = scope.ServiceProvider.GetRequiredService<LeaveTakeRepository>();

            // Buscar a otimização
            var optimization = await repository.GetByIdSingleAsync(
                parameters.TourScheduleOptimizationId,
                CancellationToken.None);

            if (optimization == null)
            {
                _logger.LogError(
                    "Otimização não encontrada. OptimizationId: {OptimizationId}",
                    parameters.TourScheduleOptimizationId);
                return;
            }

            try
            {
                // Atualizar status para InProgress
                optimization.Status = TourScheduleOptimizationStatus.InProgress;
                await repository.UpdateAsync(optimization, CancellationToken.None);

                _logger.LogInformation(
                    "Status atualizado para InProgress. OptimizationId: {OptimizationId}",
                    parameters.TourScheduleOptimizationId);

                // Executar a otimização
                var solverService = new TourScheduleSolverService(
                    dbContext,
                    repository,
                    tourScheduleRepository,
                    leaveTakeRepository);

                var assignments = await solverService.SolveAsync(parameters, CancellationToken.None);

                // Atualizar status para Completed
                optimization.Status = TourScheduleOptimizationStatus.Completed;
                await repository.UpdateAsync(optimization, CancellationToken.None);

                _logger.LogInformation(
                    "Otimização concluída com sucesso. OptimizationId: {OptimizationId}, Assignments: {AssignmentCount}",
                    parameters.TourScheduleOptimizationId,
                    assignments?.Count ?? 0);
            }
            catch (Exception ex)
            {
                // Atualizar status para Failed
                optimization.Status = TourScheduleOptimizationStatus.Failed;
                await repository.UpdateAsync(optimization, CancellationToken.None);

                _logger.LogError(
                    ex,
                    "Erro ao processar otimização. OptimizationId: {OptimizationId}",
                    parameters.TourScheduleOptimizationId);

                throw; // Re-throw para que o Hangfire marque o job como falhado
            }
        }
    }
}
