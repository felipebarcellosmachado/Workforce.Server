using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Business.Core.TourScheduleManagement.TourSchedule.Repository;
using Workforce.Domain.Core.TourScheduleManagement.TourSchedule.Entity;

namespace Workforce.Server.Controllers.Core.TourScheduleManagement.TourSchedule
{
    [ApiController]
    [Route("api/core/tour-schedule-management/tourschedule")]
    public class TourScheduleController : ControllerBase
    {
        private readonly TourScheduleRepository repository;

        public TourScheduleController(TourScheduleRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetTourScheduleById")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);

                if (entity == null)
                {
                    return NotFound($"TourSchedule com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter TourSchedule: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter TourSchedules: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter TourSchedules por EnvironmentId: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);

                if (entity == null)
                {
                    return NotFound($"TourSchedule com ID {id} não encontrado");
                }

                if (entity.EnvironmentId != environmentId)
                {
                    return NotFound($"TourSchedule com ID {id} não pertence ao Environment {environmentId}");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter TourSchedule: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule>> InsertAsync([FromBody] Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);

                return Created($"/api/core/tour-schedule-management/tourschedule/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir TourSchedule: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule>> UpdateAsync(int id, [FromBody] Domain.Core.TourScheduleManagement.TourSchedule.Entity.TourSchedule entity, CancellationToken ct = default)
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
                    return NotFound($"TourSchedule com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar TourSchedule: {ex.Message}");
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
                    return NotFound($"TourSchedule com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir TourSchedule: {ex.Message}");
            }
        }
    }
}
