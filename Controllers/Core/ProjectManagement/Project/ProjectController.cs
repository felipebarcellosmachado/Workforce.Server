using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.ProjectManagement.Project.Repository;

namespace Workforce.Server.Controllers.Core.ProjectManagement.Project
{
    [ApiController]
    [Route("api/core/project-management/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectRepository projectRepository;

        public ProjectController(ProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        [HttpGet("{id:int}", Name = "GetProjectById")]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Project.Entity.Project>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var project = await projectRepository.GetByIdAsync(id, ct);
                if (project == null)
                    return NotFound($"Project with ID {id} not found");

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.ProjectManagement.Project.Entity.Project>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var projects = await projectRepository.GetAllAsync(ct);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId:int}")]
        public async Task<ActionResult<IList<Domain.Core.ProjectManagement.Project.Entity.Project>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var projects = await projectRepository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/program/{programId:int}")]
        public async Task<ActionResult<IList<Domain.Core.ProjectManagement.Project.Entity.Project>>> GetAllByProgramIdAsync(int programId, CancellationToken ct = default)
        {
            try
            {
                var projects = await projectRepository.GetAllByProgramIdAsync(programId, ct);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.ProjectManagement.Project.Entity.Project>> InsertAsync([FromBody] Domain.Core.ProjectManagement.Project.Entity.Project entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                    return BadRequest("Project data is required");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var inserted = await projectRepository.InsertAsync(entity, ct);
                return Created($"/api/core/project-management/project/{inserted.Id}", inserted);
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
        public async Task<ActionResult<Domain.Core.ProjectManagement.Project.Entity.Project>> UpdateAsync(int id, [FromBody] Domain.Core.ProjectManagement.Project.Entity.Project entity, CancellationToken ct = default)
        {
            try
            {
                if (entity == null)
                    return BadRequest("Project data is required");

                if (id != entity.Id)
                    return BadRequest("ID mismatch between URL and body");

                var updated = await projectRepository.UpdateAsync(entity, ct);
                if (updated == null)
                    return NotFound($"Project with ID {id} not found");

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
                var deleted = await projectRepository.DeleteByIdAsync(id, ct);
                if (!deleted)
                    return NotFound($"Project with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
