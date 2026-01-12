using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.TourScheduleManagement.TourScheduleOptimization.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourScheduleOptimization;

namespace Workforce.Server.Controllers.Core.TourScheduleManagement.TourScheduleOptimization
{
    [ApiController]
    [Route("api/core/tour-schedule-management/tour-schedule-optimization")]
    public class TourScheduleOptimizationController : ControllerBase
    {
        private readonly TourScheduleOptimizationRepository repository;

        public TourScheduleOptimizationController(TourScheduleOptimizationRepository repository)
        {
            this.repository = repository;
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
    }
}
