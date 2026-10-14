using Microsoft.AspNetCore.Mvc;
using StaffingScheduleTypeEntity = Workforce.Domain.Core.StaffingScheduleManagement.StaffingScheduleType.Entity.StaffingScheduleType;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.StaffingScheduleType.Repository;

namespace Workforce.Server.Controllers.Core.StaffingScheduleManagement.StaffingScheduleType;

[ApiController]
[Route("api/core/staffing-schedule-management/staffing-schedule-types")]
public class StaffingScheduleTypeController : ControllerBase
{
    private readonly StaffingScheduleTypeRepository repository;

    public StaffingScheduleTypeController(StaffingScheduleTypeRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet("{id:int}", Name = "GetStaffingScheduleTypeById")]
    public async Task<ActionResult<StaffingScheduleTypeEntity>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IList<StaffingScheduleTypeEntity>>> GetAllAsync(CancellationToken ct = default)
    {
        return Ok(await repository.GetAllAsync(ct));
    }

    [HttpGet("all/environment/{environmentId:int}")]
    public async Task<ActionResult<IList<StaffingScheduleTypeEntity>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
    {
        return Ok(await repository.GetAllByEnvironmentIdAsync(environmentId, ct));
    }

    [HttpGet("environment/{environmentId:int}/{id:int}")]
    public async Task<ActionResult<StaffingScheduleTypeEntity>> GetByEnvironmentIdAndIdAsync(int environmentId, int id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null || entity.EnvironmentId != environmentId ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<StaffingScheduleTypeEntity>> InsertAsync([FromBody] StaffingScheduleTypeEntity entity, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var insertedEntity = await repository.InsertAsync(entity, ct);
        return Created($"{Request.Path}/{insertedEntity.Id}", insertedEntity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StaffingScheduleTypeEntity>> UpdateAsync(int id, [FromBody] StaffingScheduleTypeEntity entity, CancellationToken ct = default)
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