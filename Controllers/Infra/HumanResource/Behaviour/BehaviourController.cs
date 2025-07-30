using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.HumanResource.Behaviour.Repository;
using Workforce.Domain.Infra.HumanResource.Behaviour.Entity;

namespace Workforce.Server.Controllers.Infra.HumanResource.Behaviour
{
    [ApiController]
    [Route("api/infra/humanresource/[controller]")]
    public class BehaviourController : ControllerBase
    {
        private readonly BehaviourRepository _behaviourRepository;

        public BehaviourController(BehaviourRepository behaviourRepository)
        {
            _behaviourRepository = behaviourRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Infra.HumanResource.Behaviour.Entity.Behaviour>> GetById(int id)
        {
            try
            {
                var behaviour = await _behaviourRepository.GetById(id);
                if (behaviour == null)
                {
                    return NotFound();
                }
                return Ok(behaviour);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Infra.HumanResource.Behaviour.Entity.Behaviour>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var behaviour = await _behaviourRepository.GetById(id);
                if (behaviour == null || behaviour.EnvironmentId != environmentId)
                {
                    return NotFound();
                }
                return Ok(behaviour);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Infra.HumanResource.Behaviour.Entity.Behaviour>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var behaviours = await _behaviourRepository.GetAllByEnvironmentId(environmentId);
                return Ok(behaviours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Domain.Infra.HumanResource.Behaviour.Entity.Behaviour>>> GetAll()
        {
            try
            {
                var behaviours = await _behaviourRepository.GetAll();
                return Ok(behaviours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Infra.HumanResource.Behaviour.Entity.Behaviour>> Insert([FromBody] Domain.Infra.HumanResource.Behaviour.Entity.Behaviour behaviour)
        {
            try
            {
                if (behaviour == null)
                {
                    return BadRequest("Behaviour data is required");
                }

                var result = await _behaviourRepository.Insert(behaviour);
                if (result == null)
                {
                    return BadRequest("Failed to create behaviour");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Infra.HumanResource.Behaviour.Entity.Behaviour>> Update(int id, [FromBody] Domain.Infra.HumanResource.Behaviour.Entity.Behaviour behaviour)
        {
            try
            {
                if (behaviour == null)
                {
                    return BadRequest("Behaviour data is required");
                }

                if (id != behaviour.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _behaviourRepository.Update(behaviour);
                if (result == null)
                {
                    return NotFound("Behaviour not found or update failed");
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
                var behaviour = await _behaviourRepository.GetById(id);
                if (behaviour == null)
                {
                    return NotFound();
                }

                var deleted = await _behaviourRepository.DeleteById(id);
                if (!deleted)
                {
                    return StatusCode(500, "Failed to delete behaviour");
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