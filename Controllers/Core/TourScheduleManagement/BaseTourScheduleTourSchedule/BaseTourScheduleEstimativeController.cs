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
    [Route("api/core/tour-schedule-management/basetourschedule")]
    public class BaseTourScheduleController : ControllerBase
    {
        private readonly BaseTourScheduleRepository repository;

        public BaseTourScheduleController(BaseTourScheduleRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseTourScheduleById")]
        public async Task<ActionResult<BaseTourSchedule>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseTourSchedule com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourSchedule: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseTourSchedule>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter Demands: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<BaseTourSchedule>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                
                // Log para debug
                Console.WriteLine($"GetAllByEnvironmentIdAsync: Found {entities.Count} schedules for environment {environmentId}");
                foreach (var schedule in entities)
                {
                    var daysCount = schedule.Days?.Count ?? 0;
                    var periodsCount = schedule.Days?.SelectMany(d => d.BaseTourSchedulePeriods ?? new List<BaseTourSchedulePeriod>()).Count() ?? 0;
                    Console.WriteLine($"  Schedule {schedule.Id} ({schedule.Name}): {daysCount} days, {periodsCount} periods");
                }
                
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter Demands por EnvironmentId: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<BaseTourSchedule>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null || entity.EnvironmentId != environmentId)
                {
                    return NotFound($"BaseTourSchedule com ID {id} não encontrado no Environment {environmentId}");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourSchedule: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseTourSchedule>> InsertAsync([FromBody] BaseTourSchedule entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/tour-schedule-management/basetourschedule/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseTourSchedule: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseTourSchedule>> UpdateAsync(int id, [FromBody] BaseTourSchedule entity, CancellationToken ct = default)
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
                    return NotFound($"BaseTourSchedule com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseTourSchedule: {ex.Message}");
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
                    return NotFound($"BaseTourSchedule com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseTourSchedule: {ex.Message}");
            }
        }
    }
}
