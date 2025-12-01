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
    [Route("api/core/demand-management/basedemand")]
    public class BaseDemandController : ControllerBase
    {
        private readonly BaseDemandRepository repository;

        public BaseDemandController(BaseDemandRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseDemandById")]
        public async Task<ActionResult<BaseDemand>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseDemand com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemand: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseDemand>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemands: {ex.Message}");
            }
        }

        [HttpGet("basedemandestimative/{baseDemandEstimativeId:int}")]
        public async Task<ActionResult<IList<BaseDemand>>> GetAllByBaseDemandEstimativeIdAsync(int baseDemandEstimativeId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseDemandEstimativeIdAsync(baseDemandEstimativeId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemands por BaseDemandEstimativeId: {ex.Message}");
            }
        }

        [HttpGet("basedemandperiod/{baseDemandPeriodId:int}")]
        public async Task<ActionResult<IList<BaseDemand>>> GetAllByBaseDemandPeriodIdAsync(int baseDemandPeriodId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseDemandPeriodIdAsync(baseDemandPeriodId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemands por BaseDemandPeriodId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseDemand>> InsertAsync([FromBody] BaseDemand entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/demand-management/basedemand/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseDemand: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseDemand>> UpdateAsync(int id, [FromBody] BaseDemand entity, CancellationToken ct = default)
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
                    return NotFound($"BaseDemand com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseDemand: {ex.Message}");
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
                    return NotFound($"BaseDemand com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseDemand: {ex.Message}");
            }
        }
    }
}
