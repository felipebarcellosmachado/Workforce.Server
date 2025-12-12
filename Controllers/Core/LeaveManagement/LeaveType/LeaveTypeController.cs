using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.LeaveManagement.LeaveType.Repository;

namespace Workforce.Server.Controllers.Core.LeaveManagement.LeaveType
{
    [ApiController]
    [Route("api/core/leave-management/leave-type")]
    public class LeaveTypeController : ControllerBase
    {
        private readonly LeaveTypeRepository repository;

        public LeaveTypeController(LeaveTypeRepository repository)
        {
            this.repository = repository;
        }

        // HttpGet para GetById
        [HttpGet("{id:int}", Name = "GetLeaveTypeById")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var leaveType = await repository.GetByIdAsync(id, ct);
                if (leaveType == null)
                {
                    return NotFound($"LeaveType com ID {id} não encontrado");
                }
                return Ok(leaveType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpGet("environment/{environmentId}/leaveType/{id}") para GetByEnvironmentIdAndId
        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var leaveType = await repository.GetByIdAsync(id, ct);
                if (leaveType == null || leaveType.EnvironmentId != environmentId)
                {
                    return NotFound($"LeaveType com ID {id} não encontrado para o Environment {environmentId}");
                }
                return Ok(leaveType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpGet("all/environment/{environmentId}") para GetAllByEnvironmentId
        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var leaveTypes = await repository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(leaveTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpGet("all") para GetAll
        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var leaveTypes = await repository.GetAllAsync(ct);
                return Ok(leaveTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpPost para Insert
        [HttpPost]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType>> InsertAsync([FromBody] Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType leaveType, CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var insertedLeaveType = await repository.InsertAsync(leaveType, ct);
                return Created($"{Request.Path}/{insertedLeaveType.Id}", insertedLeaveType);
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

        // HttpPut para Update
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType>> UpdateAsync(int id, [FromBody] Domain.Core.LeaveManagement.LeaveType.Entity.LeaveType leaveType, CancellationToken ct = default)
        {
            try
            {
                if (id != leaveType.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedLeaveType = await repository.UpdateAsync(leaveType, ct);
                if (updatedLeaveType == null)
                {
                    return NotFound($"LeaveType com ID {id} não encontrado");
                }
                return Ok(updatedLeaveType);
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

        // HttpDelete para DeleteById
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var deleted = await repository.DeleteByIdAsync(id, ct);
                if (!deleted)
                {
                    return NotFound($"LeaveType com ID {id} não encontrado");
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