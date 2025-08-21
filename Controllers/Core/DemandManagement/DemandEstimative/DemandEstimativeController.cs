using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.DemandManagement.DemandEstimative.Repository;

namespace Workforce.Server.Controllers.Core.DemandManagement.DemandEstimative
{
    [ApiController]
    [Route("api/demand-management/demand-estimatives")]
    public class DemandEstimativeController : ControllerBase
    {
        private readonly DemandEstimativeRepository demandEstimativeRepository;

        public DemandEstimativeController(DemandEstimativeRepository demandEstimativeRepository)
        {
            this.demandEstimativeRepository = demandEstimativeRepository;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative>> GetByIdAsync(int id)
        {
            try
            {
                var demandEstimative = await demandEstimativeRepository.GetByIdAsync(id);
                if (demandEstimative == null)
                {
                    return NotFound($"DemandEstimative com ID {id} não encontrado");
                }
                return Ok(demandEstimative);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative>> GetByEnvironmentIdAndIdAsync(int environmentId, int id)
        {
            try
            {
                var demandEstimative = await demandEstimativeRepository.GetByIdAsync(id);
                if (demandEstimative == null || demandEstimative.EnvironmentId != environmentId)
                {
                    return NotFound($"DemandEstimative com ID {id} não encontrado para o Environment {environmentId}");
                }
                return Ok(demandEstimative);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative>>> GetAllByEnvironmentIdAsync(int environmentId)
        {
            try
            {
                var demandEstimatives = await demandEstimativeRepository.GetAllByEnvironmentIdAsync(environmentId);
                return Ok(demandEstimatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative>>> GetAllAsync()
        {
            try
            {
                var demandEstimatives = await demandEstimativeRepository.GetAllAsync();
                return Ok(demandEstimatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative>> InsertAsync([FromBody] Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative entity)
        {
            try
            {
                var insertedEntity = await demandEstimativeRepository.InsertAsync(entity);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = insertedEntity.Id }, insertedEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative>> UpdateAsync(int id, [FromBody] Domain.Core.DemandManagement.DemandEstimative.Entity.DemandEstimative entity)
        {
            try
            {
                if (id != entity.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                var updatedEntity = await demandEstimativeRepository.UpdateAsync(entity);
                if (updatedEntity == null)
                {
                    return NotFound($"DemandEstimative com ID {id} não encontrado");
                }
                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                var deleted = await demandEstimativeRepository.DeleteByIdAsync(id);
                if (!deleted)
                {
                    return NotFound($"DemandEstimative com ID {id} não encontrado");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}