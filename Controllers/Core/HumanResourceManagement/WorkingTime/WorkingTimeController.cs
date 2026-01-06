using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.WorkingTime;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.WorkingTime
{
    [ApiController]
    [Route("api/core/human_resource/[controller]")]
    public class WorkingTimeController : ControllerBase
    {
        private readonly WorkingTimeRepository _workingTimeRepository;

        public WorkingTimeController(WorkingTimeRepository workingTimeRepository)
        {
            _workingTimeRepository = workingTimeRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime>> GetById(int id)
        {
            try
            {
                var workingTime = await _workingTimeRepository.GetById(id);
                if (workingTime == null)
                {
                    return NotFound();
                }
                return Ok(workingTime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var workingTime = await _workingTimeRepository.GetById(id);
                if (workingTime == null || workingTime.EnvironmentId != environmentId)
                {
                    return NotFound();
                }
                return Ok(workingTime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var workingTimes = await _workingTimeRepository.GetAllByEnvironmentId(environmentId);
                return Ok(workingTimes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime>>> GetAll()
        {
            try
            {
                var workingTimes = await _workingTimeRepository.GetAll();
                return Ok(workingTimes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime>> Insert([FromBody] Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime workingTime)
        {
            try
            {
                var insertedWorkingTime = await _workingTimeRepository.Insert(workingTime);
                if (insertedWorkingTime == null)
                {
                    return BadRequest("Failed to insert working time");
                }
                return CreatedAtAction(nameof(GetById), new { id = insertedWorkingTime.Id }, insertedWorkingTime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime>> Update(int id, [FromBody] Domain.Core.TourScheduleManagement.WorkingTime.Entity.WorkingTime workingTime)
        {
            try
            {
                if (id != workingTime.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var updatedWorkingTime = await _workingTimeRepository.Update(workingTime);
                if (updatedWorkingTime == null)
                {
                    return NotFound();
                }
                return Ok(updatedWorkingTime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                var result = await _workingTimeRepository.DeleteById(id);
                if (!result)
                {
                    return NotFound();
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
