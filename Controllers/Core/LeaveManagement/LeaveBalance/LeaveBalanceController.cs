using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.LeaveManagement.LeaveBalance.Entiyt;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveBalance;

namespace Workforce.Server.Controllers.Core.LeaveManagement.LeaveBalance
{
    [ApiController]
    [Route("api/core/leave-management/leave-balance")]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly LeaveBalanceRepository repository;

        public LeaveBalanceController(LeaveBalanceRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetLeaveBalanceById")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance>>> GetAllAsync(CancellationToken ct)
        {
            var list = await repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct)
        {
            var list = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(list);
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct)
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null || entity.EnvironmentId != environmentId) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance>> InsertAsync([FromBody] Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance entity, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance>> UpdateAsync(int id, [FromBody] Domain.Core.LeaveManagement.LeaveBalance.Entiyt.LeaveBalance entity, CancellationToken ct)
        {
            if (id != entity.Id) return BadRequest("Id mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await repository.UpdateAsync(entity, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken ct)
        {
            var deleted = await repository.DeleteByIdAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
