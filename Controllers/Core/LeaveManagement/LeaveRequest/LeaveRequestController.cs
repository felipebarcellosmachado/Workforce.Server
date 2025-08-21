using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.LeaveManagement.LeaveRequest;

namespace Workforce.Server.Controllers.Core.LeaveManagement.LeaveRequest
{
    [ApiController]
    [Route("api/leave-requests")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly LeaveRequestRepository repository;

        public LeaveRequestController(LeaveRequestRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> GetByIdAsync(int id)
        {
            try
            {
                var leaveRequest = await repository.GetByIdAsync(id);
                if (leaveRequest == null)
                {
                    return NotFound($"LeaveRequest with ID {id} not found");
                }
                return Ok(leaveRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> GetByEnvironmentIdAndIdAsync(int environmentId, int id)
        {
            try
            {
                var leaveRequest = await repository.GetByIdAsync(id);
                if (leaveRequest == null || leaveRequest.EnvironmentId != environmentId)
                {
                    return NotFound($"LeaveRequest with ID {id} not found in environment {environmentId}");
                }
                return Ok(leaveRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>>> GetAllByEnvironmentIdAsync(int environmentId)
        {
            try
            {
                var leaveRequests = await repository.GetAllByEnvironmentIdAsync(environmentId);
                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>>> GetAllAsync()
        {
            try
            {
                var leaveRequests = await repository.GetAllAsync();
                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> InsertAsync([FromBody] Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest leaveRequest)
        {
            try
            {
                var insertedLeaveRequest = await repository.InsertAsync(leaveRequest);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = insertedLeaveRequest.Id }, insertedLeaveRequest);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> UpdateAsync(int id, [FromBody] Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest leaveRequest)
        {
            try
            {
                if (id != leaveRequest.Id)
                {
                    return BadRequest("URL ID does not match object ID");
                }

                var updatedLeaveRequest = await repository.UpdateAsync(leaveRequest);
                if (updatedLeaveRequest == null)
                {
                    return NotFound($"LeaveRequest with ID {id} not found");
                }
                return Ok(updatedLeaveRequest);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                var deleted = await repository.DeleteByIdAsync(id);
                if (!deleted)
                {
                    return NotFound($"LeaveRequest with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}