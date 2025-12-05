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
    [Route("api/core/tour-schedule-management/basetourscheduleestimative")]
    public class BaseTourScheduleEstimativeController : ControllerBase
    {
        private readonly BaseTourScheduleEstimativeRepository repository;

        public BaseTourScheduleEstimativeController(BaseTourScheduleEstimativeRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseTourScheduleEstimativeById")]
        public async Task<ActionResult<BaseTourScheduleEstimative>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null)
                {
                    return NotFound($"BaseTourScheduleEstimative com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleEstimative: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseTourScheduleEstimative>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleEstimatives: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<BaseTourScheduleEstimative>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleEstimatives por EnvironmentId: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<BaseTourScheduleEstimative>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);
                
                if (entity == null || entity.EnvironmentId != environmentId)
                {
                    return NotFound($"BaseTourScheduleEstimative com ID {id} não encontrado no Environment {environmentId}");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseTourScheduleEstimative: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseTourScheduleEstimative>> InsertAsync([FromBody] BaseTourScheduleEstimative entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados da entidade são obrigatórios");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                
                return Created($"/api/core/tour-schedule-management/basetourscheduleestimative/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir BaseTourScheduleEstimative: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseTourScheduleEstimative>> UpdateAsync(int id, [FromBody] BaseTourScheduleEstimative entity, CancellationToken ct = default)
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
                    return NotFound($"BaseTourScheduleEstimative com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar BaseTourScheduleEstimative: {ex.Message}");
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
                    return NotFound($"BaseTourScheduleEstimative com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseTourScheduleEstimative: {ex.Message}");
            }
        }
    }
}
