using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity;
using Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Dto;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourScheduleOptimization;
using Workforce.Realization.Application.Core.TourScheduleManagement.Service;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourSchedule.Repository;
using Workforce.Realization.Infrastructure.External.Db;
using Hangfire;
using Workforce.Server.Services;

namespace Workforce.Server.Controllers.Core.TourScheduleManagement.TourScheduleOptimization
{
    [ApiController]
    [Route("api/core/tour-schedule-management/tour-schedule-optimization")]
    public class TourScheduleOptimizationController : ControllerBase
    {
        private readonly TourScheduleOptimizationRepository repository;
        private readonly WorkforceDbContext dbContext;
        private readonly TourScheduleRepository tourScheduleRepository;

        public TourScheduleOptimizationController(
            TourScheduleOptimizationRepository repository,
            WorkforceDbContext dbContext,
            TourScheduleRepository tourScheduleRepository)
        {
            this.repository = repository;
            this.dbContext = dbContext;
            this.tourScheduleRepository = tourScheduleRepository;
        }

        [HttpGet("{id:int}", Name = "GetTourScheduleOptimizationById")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization>>> GetAllAsync(CancellationToken ct)
        {
            var list = await repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct)
        {
            var list = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization>> InsertAsync(Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization entity, CancellationToken ct)
        {
            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization>> UpdateAsync(int id, Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization entity, CancellationToken ct)
        {
            if (id != entity.Id) return BadRequest();
            var updated = await repository.UpdateAsync(entity, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct)
        {
            var deleted = await repository.DeleteByIdAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("solve")]
        public async Task<ActionResult<TourScheduleOptimizationJobResponse>> SolveOptimizationAsync(
            [FromBody] TourScheduleOptimizationParameters parameters,
            CancellationToken ct)
        {
            // Buscar a otimização antes de enfileirar (SEM assignments para melhor performance)
            var optimization = await repository.GetByIdSingleAsync(parameters.TourScheduleOptimizationId, ct);
            if (optimization == null)
            {
                return NotFound(new { error = "Optimization not found" });
            }

            // Definir status como Pending (será alterado para InProgress pelo background service)
            optimization.Status = TourScheduleOptimizationStatus.Pending;
            await repository.UpdateAsync(optimization, ct);

            // Enfileirar o job no Hangfire
            var jobId = BackgroundJob.Enqueue<TourScheduleOptimizationBackgroundService>(
                service => service.ProcessOptimizationAsync(parameters));

            var response = new TourScheduleOptimizationJobResponse
            {
                Message = "Optimization job enqueued successfully",
                JobId = jobId,
                OptimizationId = parameters.TourScheduleOptimizationId,
                Status = TourScheduleOptimizationStatus.Pending.ToString(),
                DashboardUrl = "/hangfire"
            };

            return Ok(response);
        }

        [HttpGet("{id:int}/status", Name = "GetTourScheduleOptimizationStatus")]
        public async Task<ActionResult<TourScheduleOptimizationStatusResponse>> GetStatusAsync(int id, CancellationToken ct)
        {
            var entity = await repository.GetByIdSingleAsync(id, ct);
            if (entity == null) return NotFound();
            
            var response = new TourScheduleOptimizationStatusResponse
            {
                Id = entity.Id,
                Status = entity.Status.ToString(),
                TourScheduleId = entity.TourScheduleId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                EnvironmentId = entity.EnvironmentId,
                DashboardUrl = "/hangfire"
            };

            return Ok(response);
        }

        [HttpPost("{id:int}/reset-status")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity.TourScheduleOptimization>> ResetStatusAsync(int id, CancellationToken ct)
        {
            // Para reset de status, não precisamos dos assignments
            var optimization = await repository.GetByIdSingleAsync(id, ct);
            if (optimization == null) return NotFound();
            
            optimization.Status = TourScheduleOptimizationStatus.Pending;
            var updated = await repository.UpdateAsync(optimization, ct);
            return Ok(updated);
        }
    }
}
