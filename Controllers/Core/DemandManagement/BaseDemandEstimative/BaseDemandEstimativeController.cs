using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.DemandManagement.BaseDemandEstimative.Repository;

namespace Workforce.Server.Controllers.Core.Demand.Management.BaseDemandEstimative
{
    [ApiController]
    [Route("api/core/demand-management/base-demand-estimatives")]
    public class BaseDemandEstimativeController : ControllerBase
    {
        private readonly BaseDemandEstimativeRepository repository;

        public BaseDemandEstimativeController(BaseDemandEstimativeRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseDemandEstimativeById")]
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var baseDemandEstimative = await repository.GetByIdAsync(id, ct);
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
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var baseDemandEstimative = await repository.GetByIdAsync(id, ct);
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
        public async Task<ActionResult<IList<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var baseDemandEstimatives = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(baseDemandEstimatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var baseDemandEstimatives = await repository.GetAllAsync(ct);
                return Ok(baseDemandEstimatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> InsertAsync([FromBody] Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative entity, CancellationToken ct = default)
        {
            try
            {
                var insertedEntity = await repository.InsertAsync(entity, ct);
                return Created($"/api/core/demand-management/base-demand-estimatives/{insertedEntity.Id}", insertedEntity);
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
        public async Task<ActionResult<Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative>> UpdateAsync(int id, [FromBody] Domain.Core.DemandManagement.BaseDemandEstimative.Entity.BaseDemandEstimative entity, CancellationToken ct = default)
        {
            try
            {
                if (id != entity.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                var updatedEntity = await repository.UpdateAsync(entity, ct);
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
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var deleted = await repository.DeleteByIdAsync(id, ct);
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