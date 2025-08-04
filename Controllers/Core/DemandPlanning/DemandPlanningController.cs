using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.DemandPlanning.Repository;
using Workforce.Domain.Core.DemandPlanning.Entity;

namespace Workforce.Server.Controllers.Core.DemandPlanning
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemandPlanningController : ControllerBase
    {
        private readonly DemandPlanningRepository _demandPlanningRepository;

        public DemandPlanningController(DemandPlanningRepository demandPlanningRepository)
        {
            _demandPlanningRepository = demandPlanningRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.DemandPlanning.Entity.DemandPlanning>> GetById(int id)
        {
            try
            {
                var demandPlanning = await _demandPlanningRepository.GetById(id);
                if (demandPlanning == null)
                {
                    return NotFound($"DemandPlanning com ID {id} não encontrado");
                }
                return Ok(demandPlanning);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter DemandPlanning: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Core.DemandPlanning.Entity.DemandPlanning>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var demandPlanning = await _demandPlanningRepository.GetById(id);
                if (demandPlanning == null || demandPlanning.EnvironmentId != environmentId)
                {
                    return NotFound($"DemandPlanning com ID {id} não encontrado no ambiente {environmentId}");
                }
                return Ok(demandPlanning);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter DemandPlanning: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Core.DemandPlanning.Entity.DemandPlanning>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var demandPlannings = await _demandPlanningRepository.GetAllByEnvironmentId(environmentId);
                return Ok(demandPlannings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter DemandPlannings: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.DemandPlanning.Entity.DemandPlanning>> Update(int id, [FromBody] Domain.Core.DemandPlanning.Entity.DemandPlanning demandPlanning)
        {
            try
            {
                if (id != demandPlanning.Id)
                {
                    return BadRequest("ID na URL não corresponde ao ID do objeto");
                }

                var updatedDemandPlanning = await _demandPlanningRepository.Update(demandPlanning);
                return Ok(updatedDemandPlanning);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar DemandPlanning: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.DemandPlanning.Entity.DemandPlanning>> Insert([FromBody] Domain.Core.DemandPlanning.Entity.DemandPlanning demandPlanning)
        {
            try
            {
                var insertedDemandPlanning = await _demandPlanningRepository.Insert(demandPlanning);
                return CreatedAtAction(nameof(GetById), new { id = insertedDemandPlanning.Id }, insertedDemandPlanning);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao inserir DemandPlanning: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                await _demandPlanningRepository.DeleteById(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir DemandPlanning: {ex.Message}");
            }
        }
    }
}