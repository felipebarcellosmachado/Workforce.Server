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

        // ─────────────────────────────────────────────────────────────────────
        // Period
        // ─────────────────────────────────────────────────────────────────────

        [HttpPost("period")]
        public async Task<ActionResult<StaffingSchedulePeriod>> InsertPeriodAsync([FromBody] StaffingSchedulePeriod period, CancellationToken ct = default)
        {
            try
            {
                if (period == null) return BadRequest("Dados do período são obrigatórios");
                var inserted = await repository.InsertPeriodAsync(period, ct);
                return Created($"/api/core/staffing-schedule-management/staffingschedule/period/{inserted.Id}", inserted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao inserir StaffingSchedulePeriod: {ex.Message}");
            }
        }

        [HttpPut("period/{id:int}")]
        public async Task<ActionResult<StaffingSchedulePeriod>> UpdatePeriodAsync(int id, [FromBody] StaffingSchedulePeriod period, CancellationToken ct = default)
        {
            try
            {
                if (period == null) return BadRequest("Dados do período são obrigatórios");
                if (id != period.Id) return BadRequest("ID não corresponde");
                var updated = await repository.UpdatePeriodAsync(period, ct);
                if (updated == null) return NotFound($"StaffingSchedulePeriod com ID {id} não encontrado");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar StaffingSchedulePeriod: {ex.Message}");
            }
        }

        [HttpDelete("period/{id:int}")]
        public async Task<ActionResult> DeletePeriodAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var result = await repository.DeletePeriodAsync(id, ct);
                if (!result) return NotFound($"StaffingSchedulePeriod com ID {id} não encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir StaffingSchedulePeriod: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Demand
        // ─────────────────────────────────────────────────────────────────────

        [HttpPost("demand")]
        public async Task<ActionResult<StaffingScheduleDemand>> InsertDemandAsync([FromBody] StaffingScheduleDemand demand, CancellationToken ct = default)
        {
            try
            {
                if (demand == null) return BadRequest("Dados da demanda são obrigatórios");
                var inserted = await repository.InsertDemandAsync(demand, ct);
                return Created($"/api/core/staffing-schedule-management/staffingschedule/demand/{inserted.Id}", inserted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao inserir StaffingScheduleDemand: {ex.Message}");
            }
        }

        [HttpPut("demand/{id:int}")]
        public async Task<ActionResult<StaffingScheduleDemand>> UpdateDemandAsync(int id, [FromBody] StaffingScheduleDemand demand, CancellationToken ct = default)
        {
            try
            {
                if (demand == null) return BadRequest("Dados da demanda são obrigatórios");
                if (id != demand.Id) return BadRequest("ID não corresponde");
                var updated = await repository.UpdateDemandAsync(demand, ct);
                if (updated == null) return NotFound($"StaffingScheduleDemand com ID {id} não encontrado");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar StaffingScheduleDemand: {ex.Message}");
            }
        }

        [HttpDelete("demand/{id:int}")]
        public async Task<ActionResult> DeleteDemandAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var result = await repository.DeleteDemandAsync(id, ct);
                if (!result) return NotFound($"StaffingScheduleDemand com ID {id} não encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir StaffingScheduleDemand: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Resource
        // ─────────────────────────────────────────────────────────────────────

        [HttpPost("resource")]
        public async Task<ActionResult<StaffingScheduleResource>> InsertResourceAsync([FromBody] StaffingScheduleResource resource, CancellationToken ct = default)
        {
            try
            {
                if (resource == null) return BadRequest("Dados do recurso são obrigatórios");
                var inserted = await repository.InsertResourceAsync(resource, ct);
                return Created($"/api/core/staffing-schedule-management/staffingschedule/resource/{inserted.Id}", inserted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao inserir StaffingScheduleResource: {ex.Message}");
            }
        }

        [HttpPut("resource/{id:int}")]
        public async Task<ActionResult<StaffingScheduleResource>> UpdateResourceAsync(int id, [FromBody] StaffingScheduleResource resource, CancellationToken ct = default)
        {
            try
            {
                if (resource == null) return BadRequest("Dados do recurso são obrigatórios");
                if (id != resource.Id) return BadRequest("ID não corresponde");
                var updated = await repository.UpdateResourceAsync(resource, ct);
                if (updated == null) return NotFound($"StaffingScheduleResource com ID {id} não encontrado");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar StaffingScheduleResource: {ex.Message}");
            }
        }

        [HttpDelete("resource/{id:int}")]
        public async Task<ActionResult> DeleteResourceAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var result = await repository.DeleteResourceAsync(id, ct);
                if (!result) return NotFound($"StaffingScheduleResource com ID {id} não encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir StaffingScheduleResource: {ex.Message}");
            }
        }

        [HttpDelete("{staffingScheduleId:int}/resources")]
        public async Task<ActionResult> DeleteAllResourcesAsync(int staffingScheduleId, CancellationToken ct = default)
        {
            try
            {
                await repository.DeleteAllResourcesAsync(staffingScheduleId, ct);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir Resources do StaffingSchedule: {ex.Message}");
            }
        }
    }
}
