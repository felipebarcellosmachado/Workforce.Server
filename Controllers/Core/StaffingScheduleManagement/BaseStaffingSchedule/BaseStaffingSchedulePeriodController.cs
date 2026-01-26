using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository;

namespace Workforce.Server.Controllers.Core.StaffingScheduleManagement.BaseStaffingSchedule
{
    [ApiController]
    [Route("api/core/staffing-schedule-management/basestaffingscheduleperiod")]
    public class BaseStaffingSchedulePeriodController : ControllerBase
    {
        private readonly BaseStaffingSchedulePeriodRepository repository;

        public BaseStaffingSchedulePeriodController(BaseStaffingSchedulePeriodRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseStaffingSchedulePeriodById")]
        public async Task<ActionResult<BaseStaffingSchedulePeriod>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseStaffingSchedulePeriod com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingSchedulePeriod: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseStaffingSchedulePeriod>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingSchedulePeriods: {ex.Message}");
            }
        }

        [HttpGet("basestaffingschedule/{baseStaffingScheduleId:int}")]
        public async Task<ActionResult<IList<BaseStaffingSchedulePeriod>>> GetAllByBaseStaffingScheduleIdAsync(int baseStaffingScheduleId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseStaffingScheduleIdAsync(baseStaffingScheduleId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingSchedulePeriods por BaseStaffingScheduleId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseStaffingSchedulePeriod>> InsertAsync([FromBody] BaseStaffingSchedulePeriod entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/staffing-schedule-management/basestaffingscheduleperiod/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseStaffingSchedulePeriod: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseStaffingSchedulePeriod>> UpdateAsync(int id, [FromBody] BaseStaffingSchedulePeriod entity, CancellationToken ct = default)
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
                    return NotFound($"BaseStaffingSchedulePeriod com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseStaffingSchedulePeriod: {ex.Message}");
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
                    return NotFound($"BaseStaffingSchedulePeriod com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseStaffingSchedulePeriod: {ex.Message}");
            }
        }
    }
}
