using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveRequest;
using Workforce.Domain.Core.LeaveManagement.LeaveRequest.Enum;

namespace Workforce.Server.Controllers.Core.LeaveManagement.LeaveRequest
{
    [ApiController]
    [Route("api/core/leave-management/leave-requests")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly LeaveRequestRepository repository;

        public LeaveRequestController(LeaveRequestRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id:int}", Name = "GetLeaveRequestById")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var leaveRequest = await repository.GetByIdAsync(id, ct);
            
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return Ok(leaveRequest);
        }

        [HttpPost("{id:int}/approve")]
        public Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> ApproveAsync(int id, CancellationToken ct = default) =>
            UpdateStatusAsync(id, LeaveRequestStatus.Approved, ct);

        [HttpPost("{id:int}/reject")]
        public Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> RejectAsync(int id, CancellationToken ct = default) =>
            UpdateStatusAsync(id, LeaveRequestStatus.Rejected, ct);

        [HttpPost("{id:int}/cancel")]
        public Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> CancelAsync(int id, CancellationToken ct = default) =>
            UpdateStatusAsync(id, LeaveRequestStatus.Cancelled, ct);

        private async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> UpdateStatusAsync(
            int id,
            LeaveRequestStatus status,
            CancellationToken ct)
        {
            var updatedEntity = await repository.UpdateStatusAsync(id, status, ct);
            return updatedEntity == null ? NotFound() : Ok(updatedEntity);
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            var leaveRequest = await repository.GetByIdAsync(id, ct);
            
            if (leaveRequest == null)
            {
                return NotFound();
            }

            if (leaveRequest.EnvironmentId != environmentId)
            {
                return BadRequest();
            }

            return Ok(leaveRequest);
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            var leaveRequests = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
            return Ok(leaveRequests);
        }

        // Keep the generic CRUD route available when no environment has been
        // restored yet. Environment-scoped callers should use the route above.
        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>>> GetAllAsync(CancellationToken ct = default)
        {
            return Ok(await repository.GetAllAsync(ct));
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> InsertAsync([FromBody] Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest entity, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.Status = LeaveRequestStatus.Required;
            entity.EvaluationDate = null;

            var insertedEntity = await repository.InsertAsync(entity, ct);
            
            return Created($"{Request.Path}/{insertedEntity.Id}", insertedEntity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> UpdateAsync(int id, [FromBody] Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest entity, CancellationToken ct = default)
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