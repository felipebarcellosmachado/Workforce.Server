using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.WorkScheduleManagement.BaseWorkSchedule.Repository;

namespace Workforce.Server.Controllers.Core.WorkScheduleManagement.BaseWorkSchedule
{
    [ApiController]
    [Route("api/core/work-schedule-management/base-work-schedules")]
    public class BaseWorkScheduleController : ControllerBase
    {
        private readonly BaseWorkScheduleRepository baseWorkScheduleRepository;

        public BaseWorkScheduleController(BaseWorkScheduleRepository baseWorkScheduleRepository)
        {
            this.baseWorkScheduleRepository = baseWorkScheduleRepository;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule>> GetByIdAsync(int id)
        {
            try
            {
                var baseWorkSchedule = await baseWorkScheduleRepository.GetByIdAsync(id);
                if (baseWorkSchedule == null)
                {
                    return NotFound($"BaseWorkSchedule com ID {id} não encontrado");
                }
                return Ok(baseWorkSchedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule>> GetByEnvironmentIdAndIdAsync(int environmentId, int id)
        {
            try
            {
                var baseWorkSchedule = await baseWorkScheduleRepository.GetByIdAsync(id);
                if (baseWorkSchedule == null || baseWorkSchedule.EnvironmentId != environmentId)
                {
                    return NotFound($"BaseWorkSchedule com ID {id} não encontrado para o Environment {environmentId}");
                }
                return Ok(baseWorkSchedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule>>> GetAllByEnvironmentIdAsync(int environmentId)
        {
            try
            {
                var baseWorkSchedules = await baseWorkScheduleRepository.GetAllByEnvironmentIdAsync(environmentId);
                return Ok(baseWorkSchedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule>>> GetAllAsync()
        {
            try
            {
                var baseWorkSchedules = await baseWorkScheduleRepository.GetAllAsync();
                return Ok(baseWorkSchedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule>> InsertAsync([FromBody] Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule entity)
        {
            try
            {
                var insertedEntity = await baseWorkScheduleRepository.InsertAsync(entity);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = insertedEntity.Id }, insertedEntity);
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
        public async Task<ActionResult<Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule>> UpdateAsync(int id, [FromBody] Domain.Core.WorkScheduleManagement.BaseWorkSchedule.Entity.BaseWorkSchedule entity)
        {
            try
            {
                if (id != entity.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                var updatedEntity = await baseWorkScheduleRepository.UpdateAsync(entity);
                if (updatedEntity == null)
                {
                    return NotFound($"BaseWorkSchedule com ID {id} não encontrado");
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
        public async Task<ActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                var deleted = await baseWorkScheduleRepository.DeleteByIdAsync(id);
                if (!deleted)
                {
                    return NotFound($"BaseWorkSchedule com ID {id} não encontrado");
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