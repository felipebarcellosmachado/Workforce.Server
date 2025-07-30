using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.HumanResource.Technique.Repository;

namespace Workforce.Server.Controllers.Infra.HumanResource.Technique
{
    [ApiController]
    [Route("api/infra/humanresource/technique")]
    public class TechniqueController : ControllerBase
    {
        private readonly TechniqueRepository _techniqueRepository;

        public TechniqueController(TechniqueRepository techniqueRepository)
        {
            _techniqueRepository = techniqueRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var technique = await _techniqueRepository.GetById(id);
            if (technique == null) return NotFound();
            return Ok(technique);
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            var technique = await _techniqueRepository.GetByEnvironmentIdAndId(environmentId, id);
            if (technique == null) return NotFound();
            return Ok(technique);
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult> GetAllByEnvironmentId(int environmentId)
        {
            var techniques = await _techniqueRepository.GetAllByEnvironmentId(environmentId);
            return Ok(techniques);
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAll()
        {
            var techniques = await _techniqueRepository.GetAll();
            return Ok(techniques);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Domain.Infra.HumanResource.Technique.Entity.Technique technique)
        {
            if (id != technique.Id) return BadRequest();
            var result = await _techniqueRepository.Update(technique);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] Domain.Infra.HumanResource.Technique.Entity.Technique technique)
        {
            var result = await _techniqueRepository.Insert(technique);
            if (result == null) return BadRequest();
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            var success = await _techniqueRepository.DeleteById(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}