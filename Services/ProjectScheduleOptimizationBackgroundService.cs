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
    /// Serviço responsável por processar otimizações de Project Scheduling em background usando Hangfire.
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
        /// Executa a otimização de Project Scheduling em background via Hangfire.
        /// Attempts=0 evita retentativas automáticas para erros de validação de dados
        /// (ex: projeto sem atividades) que não são falhas transitórias.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessOptimizationAsync(ProjectScheduleOptimizationParameters parameters)
        {
            _logger.LogInformation(
                "Iniciando otimização de Project Scheduling em background. OptimizationId: {OptimizationId}",
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
                    "Otimização de Project Scheduling concluída. OptimizationId: {OptimizationId}",
                    parameters.ProjectScheduleOptimizationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao processar otimização de Project Scheduling. OptimizationId: {OptimizationId}",
                    parameters.ProjectScheduleOptimizationId);
                throw;
            }
        }
    }
}
