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
    [Route("api/core/demand-management/basedemandday")]
    public class BaseDemandDayController : ControllerBase
    {
        private readonly BaseDemandDayRepository repository;

        public BaseDemandDayController(BaseDemandDayRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseDemandDayById")]
        public async Task<ActionResult<BaseDemandDay>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseDemandDay com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemandDay: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseDemandDay>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemandDays: {ex.Message}");
            }
        }

        [HttpGet("basedemandestimative/{baseDemandEstimativeId:int}")]
        public async Task<ActionResult<IList<BaseDemandDay>>> GetAllByBaseDemandEstimativeIdAsync(int baseDemandEstimativeId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseDemandEstimativeIdAsync(baseDemandEstimativeId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseDemandDays por BaseDemandEstimativeId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseDemandDay>> InsertAsync([FromBody] BaseDemandDay entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/demand-management/basedemandday/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseDemandDay: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseDemandDay>> UpdateAsync(int id, [FromBody] BaseDemandDay entity, CancellationToken ct = default)
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
                    return NotFound($"BaseDemandDay com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseDemandDay: {ex.Message}");
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
                    return NotFound($"BaseDemandDay com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseDemandDay: {ex.Message}");
            }
        }
    }
}
