using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Activity.Repository;

namespace Workforce.Server.Controllers.Core.ProjectManagement.Activity
{
    [ApiController]
    [Route("api/core/project-management/activity-mode")]
    public class ActivityModeController : ControllerBase
    {
        private readonly ActivityModeRepository _repository;

        public ActivityModeController(ActivityModeRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Activity.Entity.ActivityMode>> InsertAsync(
            [FromBody] Domain.Core.ProjectManagement.Activity.Entity.ActivityMode mode,
            CancellationToken ct = default)
        {
            try
            {
                if (mode == null)
                    return BadRequest("ActivityMode data is required");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _repository.InsertAsync(mode, ct);
                return Created($"/api/core/project-management/activity-mode/{created.Id}", created);
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
        public async Task<ActionResult<Domain.Core.ProjectManagement.Activity.Entity.ActivityMode>> UpdateAsync(
            int id,
            [FromBody] Domain.Core.ProjectManagement.Activity.Entity.ActivityMode mode,
            CancellationToken ct = default)
        {
            try
            {
                if (mode == null)
                    return BadRequest("ActivityMode data is required");

                if (id != mode.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _repository.UpdateAsync(mode, ct);
                if (updated == null)
                    return NotFound($"ActivityMode with ID {id} not found");

                return Ok(updated);
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
        public async Task<ActionResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var deleted = await _repository.DeleteByIdAsync(id, ct);
                if (!deleted)
                    return NotFound($"ActivityMode with ID {id} not found");

                return NoContent();
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
    }
}
