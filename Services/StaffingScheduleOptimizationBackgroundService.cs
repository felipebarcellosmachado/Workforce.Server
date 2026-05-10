using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Dto;
using Workforce.Realization.Application.Core.StaffingScheduleManagement.Service;
using Workforce.Realization.Infrastructure.External.Db;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingSchedule.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingScheduleOptimization;

namespace Workforce.Server.Services
{
    /// <summary>
    /// ServiÃ§o responsÃ¡vel por processar otimizaÃ§Ãµes de Staffing Schedule em background usando Hangfire.
    /// Paralelo a <c>TourScheduleOptimizationBackgroundService</c>.
    /// </summary>
    public class StaffingScheduleOptimizationBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StaffingScheduleOptimizationBackgroundService> _logger;

        public StaffingScheduleOptimizationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<StaffingScheduleOptimizationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Executa a otimizaÃ§Ã£o de Staffing Schedule em background via Hangfire.
        /// Attempts=0 evita retentativas automÃ¡ticas para erros de validaÃ§Ã£o de dados
        /// (ex: StaffingSchedule sem Resources) que nÃ£o sÃ£o falhas transitÃ³rias.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessOptimizationAsync(StaffingScheduleOptimizationParameters parameters)
        {
            _logger.LogInformation(
                "Iniciando otimizaÃ§Ã£o de Staffing em background. OptimizationId: {OptimizationId}",
                parameters.StaffingScheduleOptimizationId);

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
            var optimizationRepository = scope.ServiceProvider.GetRequiredService<StaffingScheduleOptimizationRepository>();
            var staffingScheduleRepository = scope.ServiceProvider.GetRequiredService<StaffingScheduleRepository>();

            try
            {
                var solverService = new StaffingScheduleSolverService(
                    dbContext,
                    optimizationRepository,
                    staffingScheduleRepository);

                await solverService.SolveAsync(parameters);

                _logger.LogInformation(
                    "OtimizaÃ§Ã£o de Staffing concluÃ­da. OptimizationId: {OptimizationId}",
                    parameters.StaffingScheduleOptimizationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao processar otimizaÃ§Ã£o de Staffing. OptimizationId: {OptimizationId}",
                    parameters.StaffingScheduleOptimizationId);
                throw;
            }
        }
    }
}
