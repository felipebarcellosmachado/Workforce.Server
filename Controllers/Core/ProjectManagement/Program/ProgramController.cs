using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Program.Repository;

namespace Workforce.Server.Controllers.Core.ProjectManagement.Program
{
    [ApiController]
    [Route("api/core/project-management/[controller]")]
    public class ProgramController : ControllerBase
    {
        private readonly ProgramRepository programRepository;

        public ProgramController(ProgramRepository programRepository)
        {
            this.programRepository = programRepository;
        }

        [HttpGet("{id:int}", Name = "GetProgramById")]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Program.Entity.Program>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var program = await programRepository.GetByIdAsync(id, ct);
                if (program == null)
                {
                    return NotFound($"Program with ID {id} not found");
                }
                return Ok(program);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.ProjectManagement.Program.Entity.Program>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var programs = await programRepository.GetAllAsync(ct);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.ProjectManagement.Program.Entity.Program>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var programs = await programRepository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId:int}/{id:int}")]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Program.Entity.Program>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
        {
            try
            {
                var program = await programRepository.GetByIdAsync(id, ct);
                if (program == null || program.EnvironmentId != environmentId)
                {
                    return NotFound($"Program with ID {id} not found in environment {environmentId}");
                }
                return Ok(program);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Program.Entity.Program>> InsertAsync([FromBody] Domain.Core.ProjectManagement.Program.Entity.Program entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Program data is required");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var inserted = await programRepository.InsertAsync(entity, ct);
                return Created($"/api/core/project-management/program/{inserted.Id}", inserted);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Program.Entity.Program>> UpdateAsync(int id, [FromBody] Domain.Core.ProjectManagement.Program.Entity.Program entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest("Program data is required");
                }

                if (id != entity.Id)
                {
                    return BadRequest("ID mismatch between URL and body");
                }

                var updated = await programRepository.UpdateAsync(entity, ct);
                if (updated == null)
                {
                    return NotFound($"Program with ID {id} not found");
                }
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var deleted = await programRepository.DeleteByIdAsync(id, ct);
                if (!deleted)
                {
                    return NotFound($"Program with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
