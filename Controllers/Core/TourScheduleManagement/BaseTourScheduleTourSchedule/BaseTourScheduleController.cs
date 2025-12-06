using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Business.Core.TourScheduleManagement.BaseTourSchedule.Repository;
using Workforce.Domain.Core.TourScheduleManagement.BaseTourSchedule.Entity;

namespace Workforce.Server.Controllers.Core.TourScheduleManagement.BaseTourScheduleTourSchedule
{
    [ApiController]
    [Route("api/core/tour-schedule-management/basetourscheduledemand")]
    public class BaseTourScheduleDemandController : ControllerBase
    {
        private readonly BaseTourScheduleDemandRepository repository;

        public BaseTourScheduleDemandController(BaseTourScheduleDemandRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseTourScheduleDemandById")]
        public async Task<ActionResult<BaseTourScheduleDemand>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseTourScheduleDemand com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleDemand: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseTourScheduleDemand>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleDemands: {ex.Message}");
            }
        }

        [HttpGet("basetourschedule/{baseTourScheduleId:int}")]
        public async Task<ActionResult<IList<BaseTourScheduleDemand>>> GetAllByBaseTourScheduleIdAsync(int baseTourScheduleId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseTourScheduleIdAsync(baseTourScheduleId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleDemands por BaseTourScheduleId: {ex.Message}");
            }
        }

        [HttpGet("basetourscheduleperiod/{baseTourSchedulePeriodId:int}")]
        public async Task<ActionResult<IList<BaseTourScheduleDemand>>> GetAllByBaseTourSchedulePeriodIdAsync(int baseTourSchedulePeriodId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseTourSchedulePeriodIdAsync(baseTourSchedulePeriodId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleDemands por BaseTourSchedulePeriodId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseTourScheduleDemand>> InsertAsync([FromBody] BaseTourScheduleDemand entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/tour-schedule-management/basetourscheduledemand/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseTourScheduleDemand: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseTourScheduleDemand>> UpdateAsync(int id, [FromBody] BaseTourScheduleDemand entity, CancellationToken ct = default)
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
                    return NotFound($"BaseTourScheduleDemand com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseTourScheduleDemand: {ex.Message}");
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
                    return NotFound($"BaseTourScheduleDemand com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseTourScheduleDemand: {ex.Message}");
            }
        }
    }
}
