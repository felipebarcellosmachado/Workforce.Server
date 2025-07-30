using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.WorkingHours;
using Workforce.Domain.Infra.WorkingHour.Entity;

namespace Workforce.Server.Controllers.Infra.WorkingHour
{
    [ApiController]
    [Route("api/infra/workinghour")]
    public class WorkingHourController : ControllerBase
    {
        private readonly WorkingHourRepository _workingHourRepository;

        public WorkingHourController(WorkingHourRepository workingHourRepository)
        {
            _workingHourRepository = workingHourRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Infra.WorkingHour.Entity.WorkingHour>> GetById(int id)
        {
            try
            {
                var workingHour = await _workingHourRepository.GetById(id);
                if (workingHour == null)
                {
                    return NotFound();
                }
                return Ok(workingHour);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Infra.WorkingHour.Entity.WorkingHour>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var workingHour = await _workingHourRepository.GetById(id);
                if (workingHour == null || workingHour.EnvironmentId != environmentId)
                {
                    return NotFound();
                }
                return Ok(workingHour);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Infra.WorkingHour.Entity.WorkingHour>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var workingHours = await _workingHourRepository.GetAllByEnvironmentId(environmentId);
                return Ok(workingHours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Infra.WorkingHour.Entity.WorkingHour>>> GetAll()
        {
            try
            {
                var workingHours = await _workingHourRepository.GetAll();
                return Ok(workingHours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Infra.WorkingHour.Entity.WorkingHour>> Insert([FromBody] Domain.Infra.WorkingHour.Entity.WorkingHour workingHour)
        {
            try
            {
                var insertedWorkingHour = await _workingHourRepository.Insert(workingHour);
                if (insertedWorkingHour == null)
                {
                    return BadRequest("Failed to insert working hour");
                }
                return CreatedAtAction(nameof(GetById), new { id = insertedWorkingHour.Id }, insertedWorkingHour);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Infra.WorkingHour.Entity.WorkingHour>> Update(int id, [FromBody] Domain.Infra.WorkingHour.Entity.WorkingHour workingHour)
        {
            try
            {
                if (id != workingHour.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var updatedWorkingHour = await _workingHourRepository.Update(workingHour);
                if (updatedWorkingHour == null)
                {
                    return NotFound();
                }
                return Ok(updatedWorkingHour);
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
                var result = await _workingHourRepository.DeleteById(id);
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