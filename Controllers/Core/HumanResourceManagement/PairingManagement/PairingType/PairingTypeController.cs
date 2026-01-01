using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.PairingManagement.PairingType.Repository;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.PairingManagement.PairingType
{
    [ApiController]
    [Route("api/core/pairing_management/pairing_type")]
    public class PairingTypeController : ControllerBase
    {
        private readonly PairingTypeRepository repository;

        public PairingTypeController(PairingTypeRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var pairingType = await repository.GetByIdAsync(id, ct);
                if (pairingType == null)
                {
                    return NotFound($"PairingType with ID {id} not found");
                }
                return Ok(pairingType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var pairingType = await repository.GetByEnvironmentIdAndIdAsync(environmentId, id, ct);
                if (pairingType == null)
                {
                    return NotFound($"PairingType with ID {id} not found for Environment {environmentId}");
                }
                return Ok(pairingType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var pairingTypes = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(pairingTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var pairingTypes = await repository.GetAllAsync(ct);
                return Ok(pairingTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType>> InsertAsync([FromBody] Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType entity, CancellationToken ct = default)
        {
            try
            {
                // Debug log
                Console.WriteLine($"PairingTypeController.InsertAsync: Received - Id={entity.Id}, EnvironmentId={entity.EnvironmentId}, Name={entity.Name}");

                // Validate model
                if (entity.EnvironmentId <= 0)
                {
                    return BadRequest($"EnvironmentId is required and must be greater than 0. Received: {entity.EnvironmentId}");
                }

                if (string.IsNullOrWhiteSpace(entity.Name))
                {
                    return BadRequest("Name is required");
                }

                var insertedPairingType = await repository.InsertAsync(entity, ct);
                
                // Construir a URL manualmente para evitar problemas com CreatedAtAction
                var locationUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/core/pairing_management/pairing_type/{insertedPairingType.Id}";
                return Created(locationUri, insertedPairingType);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"PairingTypeController.InsertAsync: InvalidOperationException - {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PairingTypeController.InsertAsync: Exception - {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType>> UpdateAsync(int id, [FromBody] Domain.Core.HumanResourceManagement.PairingManagement.PairingType.Entity.PairingType entity, CancellationToken ct = default)
        {
            try
            {
                if (id != entity.Id)
                {
                    return BadRequest("ID in URL does not match ID in body");
                }

                var updatedPairingType = await repository.UpdateAsync(entity, ct);
                if (updatedPairingType == null)
                {
                    return NotFound($"PairingType with ID {id} not found");
                }
                return Ok(updatedPairingType);
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
                var deleted = await repository.DeleteByIdAsync(id, ct);
                if (!deleted)
                {
                    return NotFound($"PairingType with ID {id} not found");
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
