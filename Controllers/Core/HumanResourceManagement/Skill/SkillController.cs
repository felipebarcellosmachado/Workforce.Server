using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Skill;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.Skill
{
    [ApiController]
    [Route("api/core/human_resource/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly SkillRepository _skillRepository;

        public SkillController(SkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Skill.Entity.Skill>> GetById(int id)
        {
            try
            {
                var skill = await _skillRepository.GetById(id);
                if (skill == null)
                {
                    return NotFound();
                }
                return Ok(skill);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Skill.Entity.Skill>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var skill = await _skillRepository.GetById(id);
                if (skill == null || skill.EnvironmentId != environmentId)
                {
                    return NotFound();
                }
                return Ok(skill);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<List<Domain.Core.HumanResourceManagement.Skill.Entity.Skill>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var skills = await _skillRepository.GetAllByEnvironmentId(environmentId);
                return Ok(skills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Domain.Core.HumanResourceManagement.Skill.Entity.Skill>>> GetAll()
        {
            try
            {
                var skills = await _skillRepository.GetAll();
                return Ok(skills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Skill.Entity.Skill>> Insert([FromBody] Domain.Core.HumanResourceManagement.Skill.Entity.Skill skill)
        {
            try
            {
                if (skill == null)
                {
                    return BadRequest("Skill data is required");
                }

                var result = await _skillRepository.Insert(skill);
                if (result == null)
                {
                    return BadRequest("Failed to create skill");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Skill.Entity.Skill>> Update(int id, [FromBody] Domain.Core.HumanResourceManagement.Skill.Entity.Skill skill)
        {
            try
            {
                if (skill == null)
                {
                    return BadRequest("Skill data is required");
                }

                if (id != skill.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _skillRepository.Update(skill);
                if (result == null)
                {
                    return NotFound("Skill not found or update failed");
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
                var skill = await _skillRepository.GetById(id);
                if (skill == null)
                {
                    return NotFound();
                }

                await _skillRepository.DeleteById(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}