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
    [Route("api/core/staffing-schedule-management/basestaffingscheduledemand")]
    public class BaseStaffingScheduleDemandController : ControllerBase
    {
        private readonly BaseStaffingScheduleDemandRepository repository;

        public BaseStaffingScheduleDemandController(BaseStaffingScheduleDemandRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseStaffingScheduleDemandById")]
        public async Task<ActionResult<BaseStaffingScheduleDemand>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseStaffingScheduleDemand com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingScheduleDemand: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseStaffingScheduleDemand>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingScheduleDemands: {ex.Message}");
            }
        }

        [HttpGet("basestaffingschedule/{baseStaffingScheduleId:int}")]
        public async Task<ActionResult<IList<BaseStaffingScheduleDemand>>> GetAllByBaseStaffingScheduleIdAsync(int baseStaffingScheduleId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseStaffingScheduleIdAsync(baseStaffingScheduleId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingScheduleDemands por BaseStaffingScheduleId: {ex.Message}");
            }
        }

        [HttpGet("basestaffingscheduleperiod/{baseStaffingSchedulePeriodId:int}")]
        public async Task<ActionResult<IList<BaseStaffingScheduleDemand>>> GetAllByBaseStaffingSchedulePeriodIdAsync(int baseStaffingSchedulePeriodId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseStaffingSchedulePeriodIdAsync(baseStaffingSchedulePeriodId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingScheduleDemands por BaseStaffingSchedulePeriodId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseStaffingScheduleDemand>> InsertAsync([FromBody] BaseStaffingScheduleDemand entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/staffing-schedule-management/basestaffingscheduledemand/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseStaffingScheduleDemand: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseStaffingScheduleDemand>> UpdateAsync(int id, [FromBody] BaseStaffingScheduleDemand entity, CancellationToken ct = default)
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
                    return NotFound($"BaseStaffingScheduleDemand com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseStaffingScheduleDemand: {ex.Message}");
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
                    return NotFound($"BaseStaffingScheduleDemand com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseStaffingScheduleDemand: {ex.Message}");
            }
        }
    }
}
