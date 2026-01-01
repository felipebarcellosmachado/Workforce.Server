using Microsoft.AspNetCore.Mvc;
using Workforce.Domain.Core.HumanResourceManagement.RiskFactor.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.RiskFactor;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.RiskFactor
{
    [ApiController]
    [Route("api/core/human-resource-management/[controller]")]
    public class RiskFactorController : ControllerBase
    {
        private readonly RiskFactorRepository riskFactorRepository;

        public RiskFactorController(RiskFactorRepository riskFactorRepository)
        {
            this.riskFactorRepository = riskFactorRepository;
        }

        [HttpGet("{id:int}", Name = "GetRiskFactorById")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.RiskFactor.Entity.RiskFactor>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var riskFactor = await riskFactorRepository.GetByIdAsync(id, ct);
                if (riskFactor == null)
                {
                    return NotFound($"RiskFactor with ID {id} not found");
                }
                return Ok(riskFactor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.RiskFactor.Entity.RiskFactor>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var riskFactors = await riskFactorRepository.GetAllAsync(ct);
                return Ok(riskFactors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.RiskFactor.Entity.RiskFactor>> InsertAsync([FromBody] Domain.Core.HumanResourceManagement.RiskFactor.Entity.RiskFactor entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("RiskFactor data is required");
                }

                var insertedEntity = await riskFactorRepository.InsertAsync(entity, ct);
                
                // Return 201 Created with the entity and Location header
                return Created($"/api/core/human-resource-management/riskfactor/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.RiskFactor.Entity.RiskFactor>> UpdateAsync(int id, [FromBody] Domain.Core.HumanResourceManagement.RiskFactor.Entity.RiskFactor entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("RiskFactor data is required");
                }

                if (id != entity.Id)
                {
                    return BadRequest("ID mismatch between URL and body");
                }

                var updatedEntity = await riskFactorRepository.UpdateAsync(entity, ct);
                if (updatedEntity == null)
                {
                    return NotFound($"RiskFactor with ID {id} not found");
                }
                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var deleted = await riskFactorRepository.DeleteByIdAsync(id, ct);
                if (!deleted)
                {
                    return NotFound($"RiskFactor with ID {id} not found");
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
