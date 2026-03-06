using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Workforce.Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Dto;
using Workforce.Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectScheduleManagement.ProjectScheduleOptimization;
using Workforce.Server.Services;

namespace Workforce.Server.Controllers.Core.ProjectScheduleManagement.ProjectScheduleOptimization
{
    [ApiController]
    [Route("api/core/project-schedule-management/project-schedule-optimization")]
    public class ProjectScheduleOptimizationController : ControllerBase
    {
        private readonly ProjectScheduleOptimizationRepository repository;

        public ProjectScheduleOptimizationController(ProjectScheduleOptimizationRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetProjectScheduleOptimizationById")]
        public async Task<ActionResult<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            var list = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(list);
        }

        [HttpGet("all/project/{projectId:int}")]
        public async Task<ActionResult<IList<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>>> GetAllByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            var list = await repository.GetAllByProjectIdAsync(projectId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>> InsertAsync(
            [FromBody] Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization entity,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>> UpdateAsync(
            int id,
            [FromBody] Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization entity,
            CancellationToken ct = default)
        {
            if (id != entity.Id) return BadRequest();
            var updated = await repository.UpdateAsync(entity, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            var deleted = await repository.DeleteByIdAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("solve")]
        public async Task<ActionResult<ProjectScheduleOptimizationJobResponse>> SolveOptimizationAsync(
            [FromBody] ProjectScheduleOptimizationParameters parameters,
            CancellationToken ct = default)
        {
            var optimization = await repository.GetByIdAsync(parameters.ProjectScheduleOptimizationId, ct);
            if (optimization == null)
                return NotFound(new { error = "Optimization not found" });

            optimization.Status = ProjectScheduleOptimizationStatus.Pending;
            await repository.UpdateAsync(optimization, ct);

            var jobId = BackgroundJob.Enqueue<ProjectScheduleOptimizationBackgroundService>(
                service => service.ProcessOptimizationAsync(parameters));

            return Ok(new ProjectScheduleOptimizationJobResponse
            {
                Message = "Optimization job enqueued successfully",
                JobId = jobId,
                OptimizationId = parameters.ProjectScheduleOptimizationId,
                Status = ProjectScheduleOptimizationStatus.Pending.ToString(),
                DashboardUrl = "/hangfire"
            });
        }

        [HttpGet("{id:int}/status", Name = "GetProjectScheduleOptimizationStatus")]
        public async Task<ActionResult<ProjectScheduleOptimizationStatusResponse>> GetStatusAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();

            var violations = (entity.Violations ?? new List<ProjectScheduleOptimizationViolation>())
                .Select(v => new ProjectScheduleOptimizationViolationResponse
                {
                    ViolationType = v.ViolationType,
                    ActivityId = v.ActivityId,
                    ActivityName = v.ActivityName,
                    PlannedEndDate = v.PlannedEndDate,
                    DeadlineDate = v.DeadlineDate,
                    RequiredResources = v.RequiredResources,
                    AllocatedResources = v.AllocatedResources,
                    LatenessAmount = v.LatenessAmount,
                    Message = v.Message
                }).ToList();

            var score = entity.Scores?.FirstOrDefault();
            ProjectScheduleOptimizationScoreResponse? scoreResponse = null;
            if (score != null)
            {
                scoreResponse = new ProjectScheduleOptimizationScoreResponse
                {
                    HardScore = score.HardScore,
                    MediumScore = score.MediumScore,
                    SoftScore = score.SoftScore,
                    IsFeasible = score.IsFeasible,
                    TotalScore = score.TotalScore,
                    TotalActivitiesCount = score.TotalActivitiesCount,
                    ActivitiesOnSchedule = score.ActivitiesOnSchedule,
                    ActivitiesAtRisk = score.ActivitiesAtRisk,
                    ActivitiesLate = score.ActivitiesLate,
                    OnScheduleRate = score.OnScheduleRate,
                    TotalMakespan = score.TotalMakespan,
                    TotalAllocatedResources = score.TotalAllocatedResources,
                    UniqueModesUsed = score.UniqueModesUsed,
                    TotalCost = score.TotalCost,
                    TotalLateness = score.TotalLateness
                };
            }

            return Ok(new ProjectScheduleOptimizationStatusResponse
            {
                Id = entity.Id,
                Status = entity.Status.ToString(),
                ProjectId = entity.ProjectId,
                TotalScheduledActivities = entity.TotalScheduledActivities,
                TotalModesSelected = entity.TotalModesSelected,
                TotalMakespan = entity.TotalMakespan,
                IsInfeasible = entity.IsInfeasible,
                TotalViolations = entity.TotalViolations,
                TotalLateness = entity.TotalLateness,
                OnScheduleRate = scoreResponse?.OnScheduleRate ?? 0m,
                DiagnosticMessage = entity.DiagnosticMessage,
                Score = scoreResponse,
                Violations = violations,
                DashboardUrl = "/hangfire"
            });
        }

        [HttpPost("{id:int}/reset-status")]
        public async Task<ActionResult<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>> ResetStatusAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();

            entity.Status = ProjectScheduleOptimizationStatus.Pending;
            var updated = await repository.UpdateAsync(entity, ct);
            return Ok(updated);
        }

        [HttpGet("{id:int}/dashboard", Name = "GetProjectScheduleOptimizationForDashboard")]
        public async Task<ActionResult<Domain.Core.ProjectScheduleManagement.ProjectScheduleOptimization.Entity.ProjectScheduleOptimization>> GetByIdForDashboardAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdWithAllocationsAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }
    }
}

