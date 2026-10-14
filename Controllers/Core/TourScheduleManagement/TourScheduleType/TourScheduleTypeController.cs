using Microsoft.AspNetCore.Mvc;
using TourScheduleTypeEntity = Workforce.Domain.Core.TourScheduleManagement.TourScheduleType.Entity.TourScheduleType;
using Workforce.Realization.Infrastructure.Persistence.Core.TourScheduleManagement.TourScheduleType.Repository;

namespace Workforce.Server.Controllers.Core.TourScheduleManagement.TourScheduleType;

[ApiController]
[Route("api/core/tour-schedule-management/tour-schedule-types")]
public class TourScheduleTypeController : ControllerBase
{
    private readonly TourScheduleTypeRepository repository;

    public TourScheduleTypeController(TourScheduleTypeRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet("{id:int}", Name = "GetTourScheduleTypeById")]
    public async Task<ActionResult<TourScheduleTypeEntity>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IList<TourScheduleTypeEntity>>> GetAllAsync(CancellationToken ct = default)
    {
        return Ok(await repository.GetAllAsync(ct));
    }

    [HttpGet("all/environment/{environmentId:int}")]
    public async Task<ActionResult<IList<TourScheduleTypeEntity>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
    {
        return Ok(await repository.GetAllByEnvironmentIdAsync(environmentId, ct));
    }

    [HttpGet("environment/{environmentId:int}/{id:int}")]
    public async Task<ActionResult<TourScheduleTypeEntity>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null || entity.EnvironmentId != environmentId ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<TourScheduleTypeEntity>> InsertAsync([FromBody] TourScheduleTypeEntity entity, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var insertedEntity = await repository.InsertAsync(entity, ct);
        return Created($"{Request.Path}/{insertedEntity.Id}", insertedEntity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TourScheduleTypeEntity>> UpdateAsync(int id, [FromBody] TourScheduleTypeEntity entity, CancellationToken ct = default)
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