using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Business.Core.HumanResourceManagement.Availability.Repository;
using Workforce.Domain.Core.HumanResourceManagement.Availability.Entity;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.Availability
{
    [ApiController]
    [Route("api/core/human-resource-management/availability")]
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityRepository _repository;

        public AvailabilityController(AvailabilityRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetAvailabilityById")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Availability.Entity.Availability>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await _repository.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Availability.Entity.Availability>>> GetAllAsync(CancellationToken ct)
        {
            var list = await _repository.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("humanresource/{humanResourceId:int}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Availability.Entity.Availability>>> GetByHumanResourceIdAsync(int humanResourceId, CancellationToken ct)
        {
            var list = await _repository.GetByHumanResourceIdAsync(humanResourceId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Availability.Entity.Availability>> InsertAsync(Domain.Core.HumanResourceManagement.Availability.Entity.Availability entity, CancellationToken ct)
        {
            var inserted = await _repository.InsertAsync(entity, ct);
            return Created($"{Request.Path}/{inserted.Id}", inserted);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Availability.Entity.Availability>> UpdateAsync(int id, Domain.Core.HumanResourceManagement.Availability.Entity.Availability entity, CancellationToken ct)
        {
            if (id != entity.Id) return BadRequest();
            var updated = await _repository.UpdateAsync(entity, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteByIdAsync(int id, CancellationToken ct)
        {
            var deleted = await _repository.DeleteByIdAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
