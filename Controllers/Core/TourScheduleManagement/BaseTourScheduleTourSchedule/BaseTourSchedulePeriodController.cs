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
    [Route("api/core/tour-schedule-management/basetourscheduleperiod")]
    public class BaseTourSchedulePeriodController : ControllerBase
    {
        private readonly BaseTourSchedulePeriodRepository repository;

        public BaseTourSchedulePeriodController(BaseTourSchedulePeriodRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseTourSchedulePeriodById")]
        public async Task<ActionResult<BaseTourSchedulePeriod>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseTourSchedulePeriod com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourSchedulePeriod: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseTourSchedulePeriod>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourSchedulePeriods: {ex.Message}");
            }
        }

        [HttpGet("basetourscheduleday/{baseTourScheduleDayId:int}")]
        public async Task<ActionResult<IList<BaseTourSchedulePeriod>>> GetAllByBaseTourScheduleDayIdAsync(int baseTourScheduleDayId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseTourScheduleDayIdAsync(baseTourScheduleDayId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourSchedulePeriods por BaseTourScheduleDayId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseTourSchedulePeriod>> InsertAsync([FromBody] BaseTourSchedulePeriod entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/tour-schedule-management/basetourscheduleperiod/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseTourSchedulePeriod: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseTourSchedulePeriod>> UpdateAsync(int id, [FromBody] BaseTourSchedulePeriod entity, CancellationToken ct = default)
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
                    return NotFound($"BaseTourSchedulePeriod com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseTourSchedulePeriod: {ex.Message}");
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
                    return NotFound($"BaseTourSchedulePeriod com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseTourSchedulePeriod: {ex.Message}");
            }
        }
    }
}
