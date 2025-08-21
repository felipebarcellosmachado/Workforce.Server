using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.HumanResourceManagement.HumanResource.Repository;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.HumanResource
{
    [ApiController]
    [Route("api/infra/[controller]")]
    public class HumanResourceController : ControllerBase
    {
        private readonly HumanResourceRepository _humanResourceRepository;

        public HumanResourceController(HumanResourceRepository humanResourceRepository)
        {
            _humanResourceRepository = humanResourceRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource>> GetById(int id)
        {
            try
            {
                var humanResource = await _humanResourceRepository.GetById(id);
                if (humanResource == null)
                {
                    return NotFound($"HumanResource com ID {id} não encontrado");
                }
                return Ok(humanResource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var humanResource = await _humanResourceRepository.GetById(id);
                if (humanResource == null || humanResource.Person?.EnvironmentId != environmentId)
                {
                    return NotFound();
                }
                return Ok(humanResource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var humanResources = await _humanResourceRepository.GetAllByEnvironmentId(environmentId);
                return Ok(humanResources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource>>> GetAll()
        {
            try
            {
                var humanResources = await _humanResourceRepository.GetAll();
                return Ok(humanResources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource>> Insert([FromBody] Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource humanResource)
        {
            try
            {
                if (humanResource == null)
                {
                    return BadRequest("HumanResource data is required");
                }

                var result = await _humanResourceRepository.Insert(humanResource);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource>> Update(int id, [FromBody] Domain.Core.HumanResourceManagement.HumanResource.Entity.HumanResource humanResource)
        {
            try
            {
                if (humanResource == null)
                {
                    return BadRequest("HumanResource data is required");
                }

                if (id != humanResource.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _humanResourceRepository.Update(humanResource);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                await _humanResourceRepository.DeleteById(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}