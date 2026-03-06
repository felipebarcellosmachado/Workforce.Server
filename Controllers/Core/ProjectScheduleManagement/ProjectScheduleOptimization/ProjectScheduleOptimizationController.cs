using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectScheduleManagement.ProjectScheduleOptimization;

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
    }
}
