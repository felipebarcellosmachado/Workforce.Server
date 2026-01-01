using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveTake;

namespace Workforce.Server.Controllers.Core.LeaveManagement.LeaveTake
{
    [ApiController]
    [Route("api/leave-takes")]
    public class LeaveTakeController : ControllerBase
    {
        private readonly LeaveTakeRepository repository;

        public LeaveTakeController(LeaveTakeRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> GetByIdAsync(int id)
        {
            try
            {
                var leaveTake = await repository.GetByIdAsync(id);
                if (leaveTake == null)
                {
                    return NotFound($"LeaveTake with ID {id} not found");
                }
                return Ok(leaveTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> GetByEnvironmentIdAndIdAsync(int environmentId, int id)
        {
            try
            {
                var leaveTake = await repository.GetByIdAsync(id);
                if (leaveTake == null || leaveTake.EnvironmentId != environmentId)
                {
                    return NotFound($"LeaveTake with ID {id} not found in environment {environmentId}");
                }
                return Ok(leaveTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>>> GetAllByEnvironmentIdAsync(int environmentId)
        {
            try
            {
                var leaveTakes = await repository.GetAllByEnvironmentIdAsync(environmentId);
                return Ok(leaveTakes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>>> GetAllAsync()
        {
            try
            {
                var leaveTakes = await repository.GetAllAsync();
                return Ok(leaveTakes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> InsertAsync([FromBody] Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake leaveTake)
        {
            try
            {
                var insertedLeaveTake = await repository.InsertAsync(leaveTake);
                return CreatedAtAction(nameof(GetByEnvironmentIdAndIdAsync), new { environmentId = insertedLeaveTake.EnvironmentId, id = insertedLeaveTake.Id }, insertedLeaveTake);
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
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake>> UpdateAsync(int id, [FromBody] Domain.Core.LeaveManagement.LeaveTake.Entity.LeaveTake leaveTake)
        {
            try
            {
                if (id != leaveTake.Id)
                {
                    return BadRequest("URL ID does not match object ID");
                }

                var updatedLeaveTake = await repository.UpdateAsync(leaveTake);
                if (updatedLeaveTake == null)
                {
                    return NotFound($"LeaveTake with ID {id} not found");
                }
                return Ok(updatedLeaveTake);
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
                    return NotFound($"LeaveTake with ID {id} not found");
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