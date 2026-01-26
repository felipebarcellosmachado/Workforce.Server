using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingSchedule.Repository;

namespace Workforce.Server.Controllers.Core.StaffingScheduleManagement.StaffingSchedule
{
    [ApiController]
    [Route("api/core/staffing-schedule-management/staffingschedule")]
    public class StaffingScheduleController : ControllerBase
    {
        private readonly StaffingScheduleRepository repository;

        public StaffingScheduleController(StaffingScheduleRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetStaffingScheduleById")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);

                if (entity == null)
                {
                    return NotFound($"StaffingSchedule com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter StaffingSchedule: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter StaffingSchedules: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter StaffingSchedules por EnvironmentId: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);

                if (entity == null)
                {
                    return NotFound($"StaffingSchedule com ID {id} não encontrado");
                }

                if (entity.EnvironmentId != environmentId)
                {
                    return NotFound($"StaffingSchedule com ID {id} não pertence ao Environment {environmentId}");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter StaffingSchedule: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule>> InsertAsync([FromBody] Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);

                return Created($"/api/core/staffing-schedule-management/staffingschedule/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir StaffingSchedule: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule>> UpdateAsync(int id, [FromBody] Domain.Core.StaffingScheduleManagement.StaffingSchedule.Entity.StaffingSchedule entity, CancellationToken ct = default)
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
                    return NotFound($"StaffingSchedule com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar StaffingSchedule: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var result = await repository.DeleteAsync(id, ct);

                if (!result)
                {
                    return NotFound($"StaffingSchedule com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir StaffingSchedule: {ex.Message}");
            }
        }
    }
}
