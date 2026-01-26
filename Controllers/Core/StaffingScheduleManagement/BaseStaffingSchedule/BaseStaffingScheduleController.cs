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
    [Route("api/core/staffing-schedule-management/basestaffingschedule")]
    public class BaseStaffingScheduleController : ControllerBase
    {
        private readonly BaseStaffingScheduleRepository repository;

        public BaseStaffingScheduleController(BaseStaffingScheduleRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseStaffingScheduleById")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseStaffingSchedule com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingSchedule: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingSchedules: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                
                // Log para debug
                Console.WriteLine($"GetAllByEnvironmentIdAsync: Found {entities.Count} schedules for environment {environmentId}");
                foreach (var schedule in entities)
                {
                    var periodsCount = schedule.Periods?.Count ?? 0;
                    Console.WriteLine($"  Schedule {schedule.Id} ({schedule.Name}): {periodsCount} periods");
                }
                
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingSchedules por EnvironmentId: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null || entity.EnvironmentId != environmentId)
                {
                    return NotFound($"BaseStaffingSchedule com ID {id} não encontrado no Environment {environmentId}");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingSchedule: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule>> InsertAsync([FromBody] Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/staffing-schedule-management/basestaffingschedule/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseStaffingSchedule: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule>> UpdateAsync(int id, [FromBody] Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity.BaseStaffingSchedule entity, CancellationToken ct = default)
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
                    return NotFound($"BaseStaffingSchedule com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseStaffingSchedule: {ex.Message}");
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
                    return NotFound($"BaseStaffingSchedule com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseStaffingSchedule: {ex.Message}");
            }
        }
    }
}
