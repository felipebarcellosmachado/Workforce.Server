using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.HumanResourceManagement.Class.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Class;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.Class
{
    [ApiController]
    [Route("api/core/human-resource-management/class")]
    public class ClassController : ControllerBase
    {
        private readonly ClassRepository repository;

        public ClassController(ClassRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetClassById")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Class.Entity.Class>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Class.Entity.Class>>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Class.Entity.Class>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            var list = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Class.Entity.Class>> InsertAsync([FromBody] Domain.Core.HumanResourceManagement.Class.Entity.Class entity, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Class.Entity.Class>> UpdateAsync(int id, [FromBody] Domain.Core.HumanResourceManagement.Class.Entity.Class entity, CancellationToken ct = default)
        {
            if (id != entity.Id) return BadRequest("Id mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
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
