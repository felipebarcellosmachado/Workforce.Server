using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.HumanResourceManagement.FunctionalUnit.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.FunctionalUnit;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.FunctionalUnit
{
    [ApiController]
    [Route("api/core/human-resource-management/functionalunit")]
    public class FunctionalUnitController : ControllerBase
    {
        private readonly FunctionalUnitRepository repository;

        public FunctionalUnitController(FunctionalUnitRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetFunctionalUnitById")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.FunctionalUnit.Entity.FunctionalUnit>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.FunctionalUnit.Entity.FunctionalUnit>>> GetAllAsync(CancellationToken ct)
        {
            var list = await repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.FunctionalUnit.Entity.FunctionalUnit>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct)
        {
            var list = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.FunctionalUnit.Entity.FunctionalUnit>> InsertAsync(Domain.Core.HumanResourceManagement.FunctionalUnit.Entity.FunctionalUnit entity, CancellationToken ct)
        {
            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.FunctionalUnit.Entity.FunctionalUnit>> UpdateAsync(int id, Domain.Core.HumanResourceManagement.FunctionalUnit.Entity.FunctionalUnit entity, CancellationToken ct)
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
