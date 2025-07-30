using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.HumanResource.JobTitle.Repository;
using Workforce.Domain.Infra.HumanResource.JobTitle.Entity;

namespace Workforce.Server.Controllers.Infra.HumanResource.JobTitle
{
    [ApiController]
    [Route("api/infra/humanresource/[controller]")]
    public class JobTitleController : ControllerBase
    {
        private readonly JobTitleRepository _jobTitleRepository;

        public JobTitleController(JobTitleRepository jobTitleRepository)
        {
            _jobTitleRepository = jobTitleRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Infra.HumanResource.JobTitle.Entity.JobTitle>> GetById(int id)
        {
            try
            {
                var jobTitle = await _jobTitleRepository.GetById(id);
                if (jobTitle == null)
                {
                    return NotFound();
                }
                return Ok(jobTitle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Infra.HumanResource.JobTitle.Entity.JobTitle>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var jobTitle = await _jobTitleRepository.GetByEnvironmentIdAndId(environmentId, id);
                if (jobTitle == null)
                {
                    return NotFound();
                }
                return Ok(jobTitle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<List<Domain.Infra.HumanResource.JobTitle.Entity.JobTitle>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var jobTitles = await _jobTitleRepository.GetAllByEnvironmentId(environmentId);
                return Ok(jobTitles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Domain.Infra.HumanResource.JobTitle.Entity.JobTitle>>> GetAll()
        {
            try
            {
                var jobTitles = await _jobTitleRepository.GetAll();
                return Ok(jobTitles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Infra.HumanResource.JobTitle.Entity.JobTitle>> Insert([FromBody] Domain.Infra.HumanResource.JobTitle.Entity.JobTitle jobTitle)
        {
            try
            {
                if (jobTitle == null)
                {
                    return BadRequest("JobTitle data is required");
                }

                var result = await _jobTitleRepository.Insert(jobTitle);
                if (result == null)
                {
                    return BadRequest("Failed to create job title");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Infra.HumanResource.JobTitle.Entity.JobTitle>> Update(int id, [FromBody] Domain.Infra.HumanResource.JobTitle.Entity.JobTitle jobTitle)
        {
            try
            {
                if (jobTitle == null)
                {
                    return BadRequest("JobTitle data is required");
                }

                if (id != jobTitle.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _jobTitleRepository.Update(jobTitle);
                if (result == null)
                {
                    return NotFound("JobTitle not found or update failed");
                }

                return Ok(result);
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
                var jobTitle = await _jobTitleRepository.GetById(id);
                if (jobTitle == null)
                {
                    return NotFound();
                }

                var deleted = await _jobTitleRepository.DeleteById(id);
                if (!deleted)
                {
                    return StatusCode(500, "Failed to delete job title");
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