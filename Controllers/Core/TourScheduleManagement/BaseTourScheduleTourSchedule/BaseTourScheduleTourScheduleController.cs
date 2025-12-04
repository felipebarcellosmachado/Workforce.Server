using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.TourScheduleManagement.BaseTourSchedule.Repository;
using Workforce.Domain.Core.TourScheduleManagement.BaseTourSchedule.Entity;

namespace Workforce.Server.Controllers.Core.TourSchedule.Management.BaseTourScheduleTourSchedule
{
    [ApiController]
    [Route("api/core/tour-schedule-management/base-tour-schedule-tour-schedules")]
    public class BaseTourScheduleTourScheduleController : ControllerBase
    {
        private readonly BaseTourScheduleRepository repository;

        public BaseTourScheduleTourScheduleController(BaseTourScheduleRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BaseTourSchedule>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var baseTourScheduleTourSchedule = await repository.GetByIdAsync(id, ct);
                if (baseTourScheduleTourSchedule == null)
                {
                    return NotFound($"BaseTourScheduleTourSchedule com ID {id} não encontrado");
                }
                return Ok(baseTourScheduleTourSchedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<BaseTourSchedule>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var baseTourScheduleTourSchedules = await repository.GetAllAsync(ct);
                return Ok(baseTourScheduleTourSchedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("basetourscheduleestimative/{baseTourScheduleEstimativeId:int}")]
        public async Task<ActionResult<IList<BaseTourSchedule>>> GetAllByBaseTourScheduleEstimativeIdAsync(int baseTourScheduleEstimativeId, CancellationToken ct = default)
        {
            try
            {
                var baseTourSchedules = await repository.GetAllByBaseTourScheduleEstimativeIdAsync(baseTourScheduleEstimativeId, ct);
                return Ok(baseTourSchedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseTourSchedule>> InsertAsync([FromBody] BaseTourSchedule entity, CancellationToken ct = default)
        {
            try
            {
                var insertedEntity = await repository.InsertAsync(entity, ct);
                return Created($"/api/core/tour-schedule-management/base-tour-schedule-tour-schedules/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseTourSchedule>> UpdateAsync(int id, [FromBody] BaseTourSchedule entity, CancellationToken ct = default)
        {
            try
            {
                if (id != entity.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                var updatedEntity = await repository.UpdateAsync(entity, ct);
                if (updatedEntity == null)
                {
                    return NotFound($"BaseTourScheduleTourSchedule com ID {id} não encontrado");
                }
                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var deleted = await repository.DeleteByIdAsync(id, ct);
                if (!deleted)
                {
                    return NotFound($"BaseTourScheduleTourSchedule com ID {id} não encontrado");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
