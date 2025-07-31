using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.WorkUnit.Repository;
using Workforce.Domain.Infra.WorkUnit.Entity;

namespace Workforce.Server.Controllers.Infra.WorkUnit
{
    [ApiController]
    [Route("api/infra/[controller]")]
    public class WorkUnitController : ControllerBase
    {
        private readonly WorkUnitRepository _workUnitRepository;

        public WorkUnitController(WorkUnitRepository workUnitRepository)
        {
            _workUnitRepository = workUnitRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Infra.WorkUnit.Entity.WorkUnit>> GetById(int id)
        {
            try
            {
                var workUnit = await _workUnitRepository.GetById(id);
                if (workUnit == null)
                {
                    return NotFound();
                }
                return Ok(workUnit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Infra.WorkUnit.Entity.WorkUnit>>> GetAll()
        {
            try
            {
                var workUnits = await _workUnitRepository.GetAll();
                return Ok(workUnits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Infra.WorkUnit.Entity.WorkUnit>> Insert([FromBody] Domain.Infra.WorkUnit.Entity.WorkUnit workUnit)
        {
            try
            {
                if (workUnit == null)
                {
                    return BadRequest("WorkUnit data is required");
                }

                var result = await _workUnitRepository.Insert(workUnit);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Infra.WorkUnit.Entity.WorkUnit>> Update(int id, [FromBody] Domain.Infra.WorkUnit.Entity.WorkUnit workUnit)
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

                var result = await _workUnitRepository.Update(workUnit);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                await _workUnitRepository.DeleteById(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}