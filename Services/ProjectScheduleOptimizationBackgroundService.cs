using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Workforce.Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Dto;
using Workforce.Realization.Application.Core.ProjectScheduleManagement.Service;
using Workforce.Realization.Infrastructure.External.Db;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Project.Repository;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectScheduleManagement.ProjectScheduleOptimization;

namespace Workforce.Server.Services
{
    /// <summary>
    /// ServiÃ§o responsÃ¡vel por processar otimizaÃ§Ãµes de Project Scheduling em background usando Hangfire.
    /// Paralelo a <c>StaffingScheduleOptimizationBackgroundService</c>.
    /// </summary>
    public class ProjectScheduleOptimizationBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProjectScheduleOptimizationBackgroundService> _logger;

        public ProjectScheduleOptimizationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ProjectScheduleOptimizationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Executa a otimizaÃ§Ã£o de Project Scheduling em background via Hangfire.
        /// Attempts=0 evita retentativas automÃ¡ticas para erros de validaÃ§Ã£o de dados
        /// (ex: projeto sem atividades) que nÃ£o sÃ£o falhas transitÃ³rias.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessOptimizationAsync(ProjectScheduleOptimizationParameters parameters)
        {
            _logger.LogInformation(
                "Iniciando otimizaÃ§Ã£o de Project Scheduling em background. OptimizationId: {OptimizationId}",
                parameters.ProjectScheduleOptimizationId);

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
            var optimizationRepository = scope.ServiceProvider.GetRequiredService<ProjectScheduleOptimizationRepository>();
            var projectRepository = scope.ServiceProvider.GetRequiredService<ProjectRepository>();

            try
            {
                var solverService = new ProjectSchedulingSolverService(
                    dbContext,
                    optimizationRepository,
                    projectRepository);

                await solverService.SolveAsync(parameters);

                _logger.LogInformation(
                    "OtimizaÃ§Ã£o de Project Scheduling concluÃ­da. OptimizationId: {OptimizationId}",
                    parameters.ProjectScheduleOptimizationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao processar otimizaÃ§Ã£o de Project Scheduling. OptimizationId: {OptimizationId}",
                    parameters.ProjectScheduleOptimizationId);
                throw;
            }
        }
    }
}
