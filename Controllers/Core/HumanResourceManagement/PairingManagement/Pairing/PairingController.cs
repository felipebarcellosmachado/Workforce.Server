using Microsoft.AspNetCore.Mvc;
using Workforce.Domain.Core.HumanResourceManagement.PairingManagement.Pairing.Dto;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.PairingManagement.Pairing;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.PairingManagement.Pairing
{
    [ApiController]
    [Route("api/core/human_resource_management/pairing_management/[controller]")]
    public class PairingController : ControllerBase
    {
        private readonly PairingRepository pairingRepository;

        public PairingController(PairingRepository pairingRepository)
        {
            this.pairingRepository = pairingRepository;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var pairing = await pairingRepository.GetByIdAsync(id, ct);
                if (pairing == null)
                {
                    return NotFound($"Pairing com ID {id} não encontrado");
                }
                return Ok(pairing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var pairing = await pairingRepository.GetByEnvironmentIdAndIdAsync(environmentId, id, ct);
                if (pairing == null)
                {
                    return NotFound($"Pairing com ID {id} não encontrado para o Environment {environmentId}");
                }
                return Ok(pairing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var pairings = await pairingRepository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(pairings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var pairings = await pairingRepository.GetAllAsync(ct);
                return Ok(pairings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("list/all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<PairingListDto>>> GetAllListDtoByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var pairings = await pairingRepository.GetAllListDtoByEnvironmentIdAsync(environmentId, ct);
                return Ok(pairings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("list/all")]
        public async Task<ActionResult<IList<PairingListDto>>> GetAllListDtoAsync(CancellationToken ct = default)
        {
            try
            {
                var pairings = await pairingRepository.GetAllListDtoAsync(ct);
                return Ok(pairings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing>> InsertAsync([FromBody] Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing pairing, CancellationToken ct = default)
        {
            try
            {
                var insertedPairing = await pairingRepository.InsertAsync(pairing, ct);
                return CreatedAtAction(nameof(GetByEnvironmentIdAndIdAsync), new { environmentId = insertedPairing.EnvironmentId, id = insertedPairing.Id }, insertedPairing);
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
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing>> UpdateAsync(int id, [FromBody] Domain.Core.HumanResourceManagement.Pairing.Pairing.Entity.Pairing pairing, CancellationToken ct = default)
        {
            try
            {
                if (id != pairing.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                var updatedPairing = await pairingRepository.UpdateAsync(pairing, ct);
                if (updatedPairing == null)
                {
                    return NotFound($"Pairing com ID {id} não encontrado");
                }
                return Ok(updatedPairing);
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
                var deleted = await pairingRepository.DeleteByIdAsync(id, ct);
                if (!deleted)
                {
                    return NotFound($"Pairing com ID {id} não encontrado");
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
