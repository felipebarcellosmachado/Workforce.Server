using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveTake;

namespace Workforce.Server.Controllers.Core.LeaveManagement.LeaveTake
{
    [ApiController]
    [Route("api/core/leave-management/leave-takes")]
    public class LeaveTakeController : ControllerBase
    {
        private readonly LeaveTakeRepository repository;

        public LeaveTakeController(LeaveTakeRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id:int}", Name = "GetLeaveTakeById")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var leaveTake = await repository.GetByIdAsync(id, ct);
            
            if (leaveTake == null)
            {
                return NotFound();
            }

            return Ok(leaveTake);
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            var leaveTake = await repository.GetByIdAsync(id, ct);
            
            if (leaveTake == null)
            {
                return NotFound();
            }

            if (leaveTake.EnvironmentId != environmentId)
            {
                return BadRequest();
            }

            return Ok(leaveTake);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            var leaveTakes = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(leaveTakes);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> InsertAsync([FromBody] Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake entity, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var insertedEntity = await repository.InsertAsync(entity, ct);
            
            return Created($"{Request.Path}/{insertedEntity.Id}", insertedEntity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> UpdateAsync(int id, [FromBody] Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake entity, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Id)
            {
                return BadRequest();
            }

            var updatedEntity = await repository.UpdateAsync(entity, ct);
            
            if (updatedEntity == null)
            {
                return NotFound();
            }

            return Ok(updatedEntity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            var deleted = await repository.DeleteByIdAsync(id, ct);
            
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}