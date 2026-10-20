using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
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
    /// ServiÃ§o responsÃ¡vel por processar otimizaÃ§Ãµes de Tour Schedule em background usando Hangfire.
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
        /// Executa a otimizaÃ§Ã£o de Tour Schedule em background.
        /// Este mÃ©todo Ã© invocado pelo Hangfire.
        /// Attempts=0 evita retentativas automÃ¡ticas para erros de validaÃ§Ã£o de dados
        /// que nÃ£o sÃ£o falhas transitÃ³rias.
        /// </summary>
        /// <param name="parameters">ParÃ¢metros de otimizaÃ§Ã£o</param>
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessOptimizationAsync(TourScheduleOptimizationParameters parameters)
        {
            _logger.LogInformation(
                "Iniciando processamento de otimizaÃ§Ã£o em background. OptimizationId: {OptimizationId}",
                parameters.TourScheduleOptimizationId);

            try
            {
                // O scope deve abranger também a leitura inicial, para que falhas
                // antes do solver caiam no catch e atualizem status para Failed.
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
                var repository = scope.ServiceProvider.GetRequiredService<TourScheduleOptimizationRepository>();
                var tourScheduleRepository = scope.ServiceProvider.GetRequiredService<TourScheduleRepository>();
                var leaveTakeRepository = scope.ServiceProvider.GetRequiredService<LeaveTakeRepository>();

                // Buscar a otimizaÃ§Ã£o
                var optimization = await repository.GetByIdSingleAsync(
                    parameters.TourScheduleOptimizationId,
                    CancellationToken.None);

                if (optimization == null)
                {
                    _logger.LogError(
                        "OtimizaÃ§Ã£o nÃ£o encontrada. OptimizationId: {OptimizationId}",
                        parameters.TourScheduleOptimizationId);
                    return;
                }

                // Usar as opções persistidas somente quando a chamada não trouxe opções.
                // A UI envia as opções atuais; sobrescrevê-las aqui fazia a reotimização
                // ignorar alterações feitas no diálogo de parâmetros.
                if (parameters.Options == null && !string.IsNullOrEmpty(optimization.OptionsJson))
                {
                    parameters.Options = JsonSerializer.Deserialize<TourScheduleOptimizationOptions>(optimization.OptionsJson);
                }

                // Executar a otimizaÃ§Ã£o.
                // O solver Ã© responsÃ¡vel por persistir status, violaÃ§Ãµes,
                // diagnÃ³sticos e totais da otimizaÃ§Ã£o.
                var solverService = new TourScheduleSolverService(
                    dbContext,
                    repository,
                    tourScheduleRepository,
                    leaveTakeRepository);

                var assignments = await solverService.SolveAsync(parameters, CancellationToken.None);

                _logger.LogInformation(
                    "OtimizaÃ§Ã£o concluÃ­da com sucesso. OptimizationId: {OptimizationId}, Assignments: {AssignmentCount}",
                    parameters.TourScheduleOptimizationId,
                    assignments?.Count ?? 0);
            }
            catch (Exception ex)
            {
                try
                {
                    using var failureScope = _serviceProvider.CreateScope();
                    var failureRepository = failureScope.ServiceProvider.GetRequiredService<TourScheduleOptimizationRepository>();
                    var failedOptimization = await failureRepository.GetByIdSingleAsync(
                        parameters.TourScheduleOptimizationId,
                        CancellationToken.None);

                    if (failedOptimization != null)
                    {
                        failedOptimization.Status = TourScheduleOptimizationStatus.Failed;
                        await failureRepository.UpdateAsync(failedOptimization, CancellationToken.None);
                    }
                }
                catch (Exception statusException)
                {
                    _logger.LogError(
                        statusException,
                        "NÃ£o foi possÃ­vel persistir Failed. OptimizationId: {OptimizationId}",
                        parameters.TourScheduleOptimizationId);
                }

                _logger.LogError(
                    ex,
                    "Erro ao processar otimizaÃ§Ã£o. OptimizationId: {OptimizationId}",
                    parameters.TourScheduleOptimizationId);

                throw; // Re-throw para que o Hangfire marque o job como falhado
            }
        }
    }
}
