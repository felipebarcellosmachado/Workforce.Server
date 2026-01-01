using Microsoft.AspNetCore.Mvc;
using Workforce.Domain.Infra.Environment.Entity;
using Workforce.Realization.Infrastructure.Persistence.Infra.Environment;

namespace Workforce.Server.Controllers.Infra.Environment
{
    [ApiController]
    [Route("api/infra/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly EnvironmentRepository _environmentRepository;

        public EnvironmentController(EnvironmentRepository environmentRepository)
        {
            _environmentRepository = environmentRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Workforce.Domain.Infra.Environment.Entity.Environment>> GetById(int id)
        {
            try
            {
                var environment = await _environmentRepository.GetById(id);
                if (environment == null)
                {
                    return NotFound();
                }
                return Ok(environment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<Workforce.Domain.Infra.Environment.Entity.Environment>> GetByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Environment name is required");
                }

                var environment = await _environmentRepository.GetByName(name);
                if (environment == null)
                {
                    return NotFound($"Environment with name '{name}' not found");
                }
                return Ok(environment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Workforce.Domain.Infra.Environment.Entity.Environment>>> GetAll()
        {
            try
            {
                var environments = await _environmentRepository.GetAll();
                return Ok(environments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IList<Workforce.Domain.Infra.Environment.Entity.Environment>>> GetAllByUserId(int userId)
        {
            try
            {
                var environments = await _environmentRepository.GetAllByUserId(userId);
                if (environments == null)
                {
                    return NotFound($"No environments found for user with ID {userId}");
                }
                return Ok(environments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Workforce.Domain.Infra.Environment.Entity.Environment>> Insert([FromBody] Workforce.Domain.Infra.Environment.Entity.Environment environment)
        {
            try
            {
                if (environment == null)
                {
                    return BadRequest("Environment data is required");
                }

                var result = await _environmentRepository.Insert(environment);
                if (result == null)
                {
                    return BadRequest("Failed to create environment");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Workforce.Domain.Infra.Environment.Entity.Environment>> Update(int id, [FromBody] Workforce.Domain.Infra.Environment.Entity.Environment environment)
        {
            try
            {
                if (environment == null)
                {
                    return BadRequest("Environment data is required");
                }

                if (id != environment.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _environmentRepository.Update(environment);
                if (result == null)
                {
                    return NotFound("Environment not found or update failed");
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
                var environment = await _environmentRepository.GetById(id);
                if (environment == null)
                {
                    return NotFound();
                }

                var deleted = await _environmentRepository.DeleteById(id);
                if (!deleted)
                {
                    return StatusCode(500, "Failed to delete environment");
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