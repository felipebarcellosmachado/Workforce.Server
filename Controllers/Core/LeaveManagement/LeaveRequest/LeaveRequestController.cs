using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.LeaveManagement.LeaveRequest.Repository;

namespace Workforce.Server.Controllers.Core.LeaveManagement.LeaveRequest
{
    [ApiController]
    [Route("api/core/leave-management/leave-requests")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly LeaveRequestRepository leaveRequestRepository;

        public LeaveRequestController(LeaveRequestRepository leaveRequestRepository)
        {
            this.leaveRequestRepository = leaveRequestRepository ?? throw new ArgumentNullException(nameof(leaveRequestRepository));
        }

        [HttpGet("{id:int}", Name = "GetLeaveRequestById")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var leaveRequest = await leaveRequestRepository.GetByIdAsync(id, ct);
                
                if (leaveRequest == null)
                {
                    return NotFound($"LeaveRequest com ID {id} não encontrado");
                }

                return Ok(leaveRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter LeaveRequest: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var leaveRequest = await leaveRequestRepository.GetByIdAsync(id, ct);
                
                if (leaveRequest == null)
                {
                    return NotFound($"LeaveRequest com ID {id} não encontrado");
                }

                if (leaveRequest.EnvironmentId != environmentId)
                {
                    return BadRequest($"LeaveRequest com ID {id} não pertence ao Environment {environmentId}");
                }

                return Ok(leaveRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter LeaveRequest: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var leaveRequests = await leaveRequestRepository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter LeaveRequests: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> InsertAsync([FromBody] Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados do LeaveRequest são obrigatórios");
                }

                if (entity.EnvironmentId <= 0)
                {
                    return BadRequest("EnvironmentId inválido");
                }

                if (entity.LeaveTypeId <= 0)
                {
                    return BadRequest("LeaveTypeId inválido");
                }

                if (entity.HumanResourceId <= 0)
                {
                    return BadRequest("HumanResourceId inválido");
                }

                if (string.IsNullOrWhiteSpace(entity.Description))
                {
                    return BadRequest("Description é obrigatória");
                }

                var insertedEntity = await leaveRequestRepository.InsertAsync(entity, ct);
                
                // Usar Created() com URI explícita para evitar erro de rota
                return Created($"/api/core/leave-management/leave-requests/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao inserir LeaveRequest: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest>> UpdateAsync(int id, [FromBody] Domain.Core.LeaveManagement.LeaveRequest.Entity.LeaveRequest entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Dados do LeaveRequest são obrigatórios");
                }

                if (id != entity.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do LeaveRequest");
                }

                if (entity.EnvironmentId <= 0)
                {
                    return BadRequest("EnvironmentId inválido");
                }

                if (entity.LeaveTypeId <= 0)
                {
                    return BadRequest("LeaveTypeId inválido");
                }

                if (entity.HumanResourceId <= 0)
                {
                    return BadRequest("HumanResourceId inválido");
                }

                if (string.IsNullOrWhiteSpace(entity.Description))
                {
                    return BadRequest("Description é obrigatória");
                }

                var updatedEntity = await leaveRequestRepository.UpdateAsync(entity, ct);
                
                if (updatedEntity == null)
                {
                    return NotFound($"LeaveRequest com ID {id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar LeaveRequest: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var deleted = await leaveRequestRepository.DeleteByIdAsync(id, ct);
                
                if (!deleted)
                {
                    return NotFound($"LeaveRequest com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao excluir LeaveRequest: {ex.Message}");
            }
        }
    }
}