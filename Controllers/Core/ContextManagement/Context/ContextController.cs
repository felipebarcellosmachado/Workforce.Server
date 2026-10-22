using Microsoft.AspNetCore.Mvc;
using Entity = Workforce.Domain.Core.ContextManagement.Context.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.ContextManagement.Context;

namespace Workforce.Server.Controllers.Core.ContextManagement.Context
{
    [ApiController]
    [Route("api/core/context-management/contexts")]
    public class ContextController : ControllerBase
    {
        private readonly ContextRepository repository;

        public ContextController(ContextRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetContextById")]
        public async Task<ActionResult<Entity.Context>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            return entity is null ? NotFound() : Ok(entity);
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Entity.Context>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            var entity = await repository.GetByEnvironmentIdAndIdAsync(environmentId, id, ct);
            return entity is null ? NotFound() : Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Entity.Context>>> GetAllAsync(CancellationToken ct = default)
        {
            return Ok(await repository.GetAllAsync(ct));
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Entity.Context>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            return Ok(await repository.GetAllByEnvironmentIdAsync(environmentId, ct));
        }

        [HttpPost]
        public async Task<ActionResult<Entity.Context>> InsertAsync([FromBody] Entity.Context entity, CancellationToken ct = default)
        {
            if (entity.EnvironmentId <= 0)
            {
                ModelState.AddModelError(nameof(entity.EnvironmentId), "EnvironmentId must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Entity.Context>> UpdateAsync(int id, [FromBody] Entity.Context entity, CancellationToken ct = default)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = await repository.UpdateAsync(entity, ct);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            var deleted = await repository.DeleteByIdAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
