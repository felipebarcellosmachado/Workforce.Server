using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.HumanResourceManagement.Holiday.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Holiday;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.Holiday
{
    [ApiController]
    [Route("api/core/human-resource-management/holiday")]
    public class HolidayController : ControllerBase
    {
        private readonly HolidayRepository repository;

        public HolidayController(HolidayRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetHolidayById")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Holiday.Entity.Holiday>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Holiday.Entity.Holiday>>> GetAllAsync(CancellationToken ct)
        {
            var list = await repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Holiday.Entity.Holiday>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct)
        {
            var list = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Holiday.Entity.Holiday>> InsertAsync(Domain.Core.HumanResourceManagement.Holiday.Entity.Holiday entity, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Holiday.Entity.Holiday>> UpdateAsync(int id, Domain.Core.HumanResourceManagement.Holiday.Entity.Holiday entity, CancellationToken ct)
        {
            if (id != entity.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
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
