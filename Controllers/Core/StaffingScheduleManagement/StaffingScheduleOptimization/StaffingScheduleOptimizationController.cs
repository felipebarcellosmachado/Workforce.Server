using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleOptimization.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingScheduleOptimization;

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
    }
}
