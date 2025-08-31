using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.DemandManagement.BaseDemandEstimative.Repository;

namespace Workforce.Server.Controllers.Core.Demand.Management.BaseDemandEstimative
{
    [ApiController]
    [Route("api/core/demand-management/base-demand-estimatives")]
    public class BaseDemandEstimativeController : ControllerBase
    {
        private readonly BaseDemandEstimativeRepository baseDemandEstimativeRepository;

        public BaseDemandEstimativeController(BaseDemandEstimativeRepository baseDemandEstimativeRepository)
        {
            this.baseDemandEstimativeRepository = baseDemandEstimativeRepository;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> GetByIdAsync(int id)
        {
            try
            {
                var baseDemandEstimative = await baseDemandEstimativeRepository.GetByIdAsync(id);
                if (baseDemandEstimative == null)
                {
                    return NotFound($"BaseDemandEstimative com ID {id} não encontrado");
                }
                return Ok(baseDemandEstimative);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> GetByEnvironmentIdAndIdAsync(int environmentId, int id)
        {
            try
            {
                var baseDemandEstimative = await baseDemandEstimativeRepository.GetByIdAsync(id);
                if (baseDemandEstimative == null || baseDemandEstimative.EnvironmentId != environmentId)
                {
                    return NotFound($"BaseDemandEstimative com ID {id} não encontrado para o Environment {environmentId}");
                }
                return Ok(baseDemandEstimative);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>>> GetAllByEnvironmentIdAsync(int environmentId)
        {
            try
            {
                var baseDemandEstimatives = await baseDemandEstimativeRepository.GetAllByEnvironmentIdAsync(environmentId);
                return Ok(baseDemandEstimatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>>> GetAllAsync()
        {
            try
            {
                var baseDemandEstimatives = await baseDemandEstimativeRepository.GetAllAsync();
                return Ok(baseDemandEstimatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> InsertAsync([FromBody] Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative entity)
        {
            try
            {
                var insertedEntity = await baseDemandEstimativeRepository.InsertAsync(entity);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = insertedEntity.Id }, insertedEntity);
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

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> UpdateAsync(int id, [FromBody] Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative entity)
        {
            try
            {
                if (id != entity.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                var updatedEntity = await baseDemandEstimativeRepository.UpdateAsync(entity);
                if (updatedEntity == null)
                {
                    return NotFound($"BaseDemandEstimative com ID {id} não encontrado");
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
                var deleted = await baseDemandEstimativeRepository.DeleteByIdAsync(id);
                if (!deleted)
                {
                    return NotFound($"BaseDemandEstimative com ID {id} não encontrado");
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