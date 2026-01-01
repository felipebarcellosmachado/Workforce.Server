using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.FacilityManagement.WorkUnit.Repository;

namespace Workforce.Server.Controllers.Core.FacilityManagement.WorkUnit
{
    [ApiController]
    [Route("api/core/workunit")]
    public class WorkUnitController : ControllerBase
    {
        private readonly WorkUnitRepository _workUnitRepository;

        public WorkUnitController(WorkUnitRepository workUnitRepository)
        {
            _workUnitRepository = workUnitRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.FacilityManagement.WorkUnit.Entity.WorkUnit>> GetByIdAsync(int id)
        {
            try
            {
                var workUnit = await _workUnitRepository.GetByIdAsync(id);
                if (workUnit == null)
                {
                    return NotFound($"WorkUnit with ID {id} not found");
                }
                return Ok(workUnit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.FacilityManagement.WorkUnit.Entity.WorkUnit>>> GetAllAsync()
        {
            try
            {
                var workUnits = await _workUnitRepository.GetAllAsync();
                return Ok(workUnits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.FacilityManagement.WorkUnit.Entity.WorkUnit>> InsertAsync([FromBody] Domain.Core.FacilityManagement.WorkUnit.Entity.WorkUnit workUnit)
        {
            try
            {
                if (workUnit == null)
                {
                    return BadRequest("WorkUnit data is required");
                }

                var result = await _workUnitRepository.InsertAsync(workUnit);
                if (result == null)
                {
                    return BadRequest("Failed to create WorkUnit");
                }

                return Created($"/api/core/workunit/{result.Id}", result);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.FacilityManagement.WorkUnit.Entity.WorkUnit>> UpdateAsync(int id, [FromBody] Domain.Core.FacilityManagement.WorkUnit.Entity.WorkUnit workUnit)
        {
            try
            {
                if (workUnit == null)
                {
                    return BadRequest("WorkUnit data is required");
                }

                if (id != workUnit.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _workUnitRepository.UpdateAsync(workUnit);
                if (result == null)
                {
                    return NotFound($"WorkUnit with ID {id} not found or update failed");
                }

                return Ok(result);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                var deleted = await _workUnitRepository.DeleteByIdAsync(id);
                if (!deleted)
                {
                    return NotFound($"WorkUnit with ID {id} not found");
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