using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.CompetenceLevel.Repository;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.CompetenceLevel
{
    [ApiController]
    [Route("api/core/human_resource/[controller]")]
    public class CompetenceLevelController : ControllerBase
    {
        private readonly CompetenceLevelRepository _competenceLevelRepository;

        public CompetenceLevelController(CompetenceLevelRepository competenceLevelRepository)
        {
            _competenceLevelRepository = competenceLevelRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel>> GetById(int id)
        {
            try
            {
                var competenceLevel = await _competenceLevelRepository.GetById(id);
                if (competenceLevel == null)
                {
                    return NotFound();
                }
                return Ok(competenceLevel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var competenceLevel = await _competenceLevelRepository.GetById(id);
                if (competenceLevel == null || competenceLevel.EnvironmentId != environmentId)
                {
                    return NotFound();
                }
                return Ok(competenceLevel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel>>> GetAll()
        {
            try
            {
                var competenceLevels = await _competenceLevelRepository.GetAll();
                return Ok(competenceLevels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var competenceLevels = await _competenceLevelRepository.GetAllByEnvironmentId(environmentId);
                return Ok(competenceLevels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel>> Insert([FromBody] Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel competenceLevel)
        {
            try
            {
                if (competenceLevel == null)
                {
                    return BadRequest("CompetenceLevel data is required");
                }

                var result = await _competenceLevelRepository.Insert(competenceLevel);
                if (result == null)
                {
                    return BadRequest("Failed to create competence level");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel>> Update(int id, [FromBody] Domain.Core.HumanResourceManagement.CompetenceLevel.Entity.CompetenceLevel competenceLevel)
        {
            try
            {
                if (competenceLevel == null)
                {
                    return BadRequest("CompetenceLevel data is required");
                }

                if (id != competenceLevel.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _competenceLevelRepository.Update(competenceLevel);
                if (result == null)
                {
                    return NotFound("CompetenceLevel not found or update failed");
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
                var competenceLevel = await _competenceLevelRepository.GetById(id);
                if (competenceLevel == null)
                {
                    return NotFound();
                }

                var deleted = await _competenceLevelRepository.DeleteById(id);
                if (!deleted)
                {
                    return StatusCode(500, "Failed to delete competence level");
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