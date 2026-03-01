using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Dto;
using Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingScheduleOptimization;
using Workforce.Server.Services;

namespace Workforce.Server.Controllers.Core.StaffingScheduleManagement.StaffingScheduleOptimization
{
    [ApiController]
    [Route("api/core/staffing-schedule-management/staffing-schedule-optimization")]
    public class StaffingScheduleOptimizationController : ControllerBase
    {
        private readonly StaffingScheduleOptimizationRepository repository;

        public StaffingScheduleOptimizationController(StaffingScheduleOptimizationRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetStaffingScheduleOptimizationById")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization>>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            var list = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(list);
        }

        [HttpGet("{id:int}/single")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization>> GetByIdSingleAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdSingleAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization>> InsertAsync([FromBody] Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization entity, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization>> UpdateAsync(int id, [FromBody] Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization entity, CancellationToken ct = default)
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
        public async Task<ActionResult<StaffingScheduleOptimizationJobResponse>> SolveOptimizationAsync(
            [FromBody] StaffingScheduleOptimizationParameters parameters,
            CancellationToken ct = default)
        {
            var optimization = await repository.GetByIdSingleAsync(parameters.StaffingScheduleOptimizationId, ct);
            if (optimization == null)
                return NotFound(new { error = "Optimization not found" });

            optimization.Status = StaffingScheduleOptimizationStatus.Pending;
            await repository.UpdateAsync(optimization, ct);

            var jobId = BackgroundJob.Enqueue<StaffingScheduleOptimizationBackgroundService>(
                service => service.ProcessOptimizationAsync(parameters));

            return Ok(new StaffingScheduleOptimizationJobResponse
            {
                Message = "Optimization job enqueued successfully",
                JobId = jobId,
                OptimizationId = parameters.StaffingScheduleOptimizationId,
                Status = StaffingScheduleOptimizationStatus.Pending.ToString(),
                DashboardUrl = "/hangfire"
            });
        }

        [HttpGet("{id:int}/status", Name = "GetStaffingScheduleOptimizationStatus")]
        public async Task<ActionResult<StaffingScheduleOptimizationStatusResponse>> GetStatusAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdSingleAsync(id, ct);
            if (entity == null) return NotFound();

            var violations = (entity.Violations ?? new List<StaffingScheduleOptimizationViolation>())
                .Select(v => new StaffingScheduleOptimizationViolationResponse
                {
                    ViolationType = v.ViolationType,
                    DemandId = v.DemandId,
                    Date = v.Date,
                    PeriodId = v.PeriodId,
                    PeriodName = v.PeriodName,
                    WorkUnitId = v.WorkUnitId,
                    WorkUnitName = v.WorkUnitName,
                    JobTitleId = v.JobTitleId,
                    JobTitleName = v.JobTitleName,
                    RequiredCount = v.RequiredCount,
                    AllocatedCount = v.AllocatedCount,
                    DeficitAmount = v.DeficitAmount,
                    Message = v.Message
                }).ToList();

            var score = entity.Scores?.FirstOrDefault();
            StaffingScheduleOptimizationScoreResponse? scoreResponse = null;
            if (score != null)
            {
                scoreResponse = new StaffingScheduleOptimizationScoreResponse
                {
                    HardScore = score.HardScore,
                    MediumScore = score.MediumScore,
                    SoftScore = score.SoftScore,
                    IsFeasible = score.IsFeasible,
                    TotalScore = score.TotalScore,
                    TotalDemandsCount = score.TotalDemandsCount,
                    FullyCoveredDemands = score.FullyCoveredDemands,
                    PartiallyCoveredDemands = score.PartiallyCoveredDemands,
                    UncoveredDemands = score.UncoveredDemands,
                    CoverageRate = score.CoverageRate,
                    TotalAllocatedWorkers = score.TotalAllocatedWorkers,
                    UniqueProfilesUsed = score.UniqueProfilesUsed,
                    TotalCost = score.TotalCost,
                    TotalDeficit = score.TotalDeficit
                };
            }

            return Ok(new StaffingScheduleOptimizationStatusResponse
            {
                Id = entity.Id,
                Status = entity.Status.ToString(),
                StaffingScheduleId = entity.StaffingScheduleId,
                EnvironmentId = entity.EnvironmentId,
                TotalAllocatedWorkers = entity.TotalAllocatedWorkers,
                TotalProfilesUsed = entity.TotalProfilesUsed,
                IsInfeasible = entity.IsInfeasible,
                TotalViolations = entity.TotalViolations,
                TotalDeficit = entity.TotalDeficit,
                CoverageRate = score?.CoverageRate ?? 0m,
                DiagnosticMessage = entity.DiagnosticMessage,
                Score = scoreResponse,
                Violations = violations,
                DashboardUrl = "/hangfire"
            });
        }

        [HttpPost("{id:int}/reset-status")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity.StaffingScheduleOptimization>> ResetStatusAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdSingleAsync(id, ct);
            if (entity == null) return NotFound();

            entity.Status = StaffingScheduleOptimizationStatus.Pending;
            var updated = await repository.UpdateAsync(entity, ct);
            return Ok(updated);
        }
    }
}
