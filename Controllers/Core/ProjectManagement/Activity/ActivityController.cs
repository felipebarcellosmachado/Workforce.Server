using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Activity.Repository;

namespace Workforce.Server.Controllers.Core.ProjectManagement.Activity
{
    [ApiController]
    [Route("api/core/project-management/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityRepository activityRepository;

        public ActivityController(ActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository;
        }

        [HttpGet("{id:int}", Name = "GetActivityById")]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Activity.Entity.Activity>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var activity = await activityRepository.GetByIdAsync(id, ct);
                if (activity == null)
                    return NotFound($"Activity with ID {id} not found");

                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.ProjectManagement.Activity.Entity.Activity>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var activities = await activityRepository.GetAllAsync(ct);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/project/{projectId:int}")]
        public async Task<ActionResult<IList<Domain.Core.ProjectManagement.Activity.Entity.Activity>>> GetAllByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            try
            {
                var activities = await activityRepository.GetAllByProjectIdAsync(projectId, ct);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Activity.Entity.Activity>> InsertAsync([FromBody] Domain.Core.ProjectManagement.Activity.Entity.Activity entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                    return BadRequest("Activity data is required");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var inserted = await activityRepository.InsertAsync(entity, ct);
                return Created($"/api/core/project-management/activity/{inserted.Id}", inserted);
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
        public async Task<ActionResult<Domain.Core.ProjectManagement.Activity.Entity.Activity>> UpdateAsync(int id, [FromBody] Domain.Core.ProjectManagement.Activity.Entity.Activity entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                    return BadRequest("Activity data is required");

                if (id != entity.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await activityRepository.UpdateAsync(entity, ct);
                if (updated == null)
                    return NotFound($"Activity with ID {id} not found");

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
                var deleted = await activityRepository.DeleteByIdAsync(id, ct);
                if (!deleted)
                    return NotFound($"Activity with ID {id} not found");

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

        [HttpPut("{id:int}/predecessors")]
        public async Task<ActionResult> UpdatePredecessorsAsync(int id, [FromBody] IList<int> predecessorIds, CancellationToken ct = default)
        {
            try
            {
                await activityRepository.UpdatePredecessorsAsync(id, predecessorIds, ct);
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

        [HttpPut("{id:int}/dates")]
        public async Task<ActionResult> UpdateDatesAsync(int id, [FromBody] ActivityDatesDto dto, CancellationToken ct = default)
        {
            try
            {
                await activityRepository.UpdateDatesAsync(id, dto.StartDate, dto.EndDate, ct);
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

        public record ActivityDatesDto(DateTime? StartDate, DateTime? EndDate);
    }
}
