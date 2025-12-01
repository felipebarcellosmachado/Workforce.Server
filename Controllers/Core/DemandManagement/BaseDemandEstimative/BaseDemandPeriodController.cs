using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Business.Core.DemandManagement.BaseDemandEstimative.Repository;
using Workforce.Domain.Core.DemandManagement.BaseDemandEstimative.Entity;

namespace Workforce.Server.Controllers.Core.DemandManagement.BaseDemandEstimative
{
    [ApiController]
    [Route("api/core/demand-management/basedemandperiod")]
    public class BaseDemandPeriodController : ControllerBase
    {
        private readonly BaseDemandPeriodRepository repository;

        public BaseDemandPeriodController(BaseDemandPeriodRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseDemandPeriodById")]
        public async Task<ActionResult<BaseDemandPeriod>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseDemandPeriod com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemandPeriod: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseDemandPeriod>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemandPeriods: {ex.Message}");
            }
        }

        [HttpGet("basedemandday/{baseDemandDayId:int}")]
        public async Task<ActionResult<IList<BaseDemandPeriod>>> GetAllByBaseDemandDayIdAsync(int baseDemandDayId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseDemandDayIdAsync(baseDemandDayId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemandPeriods por BaseDemandDayId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseDemandPeriod>> InsertAsync([FromBody] BaseDemandPeriod entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/demand-management/basedemandperiod/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseDemandPeriod: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseDemandPeriod>> UpdateAsync(int id, [FromBody] BaseDemandPeriod entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                if (id != entity.Id)
                {
                    return BadRequest("ID da rota não corresponde ao ID da entidade");
                }

                var updatedEntity = await repository.UpdateAsync(entity, ct);
                
                if (updatedEntity == null)
                {
                    return NotFound($"BaseDemandPeriod com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseDemandPeriod: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var result = await repository.DeleteByIdAsync(id, ct);
                
                if (!result)
                {
                    return NotFound($"BaseDemandPeriod com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseDemandPeriod: {ex.Message}");
            }
        }
    }
}
