using Microsoft.AspNetCore.Mvc;
using WorkingTimeTypeEntity = Workforce.Domain.Core.WorkManagement.WorkingTime.Entity.WorkingTimeType;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.WorkingTimeType;

namespace Workforce.Server.Controllers.Core.WorkManagement.WorkingTimeType;

[ApiController]
[Route("api/core/work-management/working-time-types")]
public class WorkingTimeTypeController : ControllerBase
{
    private readonly WorkingTimeTypeRepository repository;

    public WorkingTimeTypeController(WorkingTimeTypeRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet("{id:int}", Name = "GetWorkingTimeTypeById")]
    public async Task<ActionResult<WorkingTimeTypeEntity>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IList<WorkingTimeTypeEntity>>> GetAllAsync(CancellationToken ct = default)
    {
        return Ok(await repository.GetAllAsync(ct));
    }

    [HttpGet("all/environment/{environmentId:int}")]
    public async Task<ActionResult<IList<WorkingTimeTypeEntity>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
    {
        return Ok(await repository.GetAllByEnvironmentIdAsync(environmentId, ct));
    }

    [HttpPost]
    public async Task<ActionResult<WorkingTimeTypeEntity>> InsertAsync([FromBody] WorkingTimeTypeEntity entity, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var insertedEntity = await repository.InsertAsync(entity, ct);
        return Created($"{Request.Path}/{insertedEntity.Id}", insertedEntity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkingTimeTypeEntity>> UpdateAsync(int id, [FromBody] WorkingTimeTypeEntity entity, CancellationToken ct = default)
    {
        if (id != entity.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedEntity = await repository.UpdateAsync(entity, ct);
        return updatedEntity is null ? NotFound() : Ok(updatedEntity);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteByIdAsync(int id, CancellationToken ct = default)
    {
        return await repository.DeleteByIdAsync(id, ct) ? NoContent() : NotFound();
    }
}
