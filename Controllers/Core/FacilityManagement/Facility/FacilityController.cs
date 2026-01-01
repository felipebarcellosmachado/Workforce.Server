using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.FacilityManagement.Facility;
using FacilityEntity = Workforce.Domain.Core.FacilityManagement.Facility.Entity.Facility;

namespace Workforce.Server.Controllers.Core.FacilityManagement.Facility
{
    [ApiController]
    [Route("api/infra/role/[controller]")]
    public class FacilityController : ControllerBase
    {
        private readonly FacilityRepository _facilityRepository;
        private readonly ILogger<FacilityController> _logger;

        public FacilityController(FacilityRepository facilityRepository, ILogger<FacilityController> logger)
        {
            _facilityRepository = facilityRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityEntity>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting facility by ID: {Id}", id);
                var facility = await _facilityRepository.GetById(id);
                if (facility == null)
                {
                    _logger.LogWarning("Facility not found for ID: {Id}", id);
                    return NotFound();
                }
                _logger.LogInformation("Successfully retrieved facility with ID: {Id}", id);
                return Ok(facility);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting facility by ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<FacilityEntity>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all facilities");
                var facilities = await _facilityRepository.GetAll();
                _logger.LogInformation("Successfully retrieved {Count} facilities", facilities.Count);
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all facilities");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}")]
        public async Task<ActionResult<List<FacilityEntity>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                _logger.LogInformation("Getting all facilities for environment ID: {EnvironmentId}", environmentId);
                var facilities = await _facilityRepository.GetAllByEnvironmentId(environmentId);
                _logger.LogInformation("Successfully retrieved {Count} facilities for environment ID: {EnvironmentId}", facilities.Count, environmentId);
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all facilities for environment ID: {EnvironmentId}", environmentId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FacilityEntity>> Insert([FromBody] FacilityEntity facility)
        {
            try
            {
                _logger.LogInformation("Creating new facility with name: {Name}", facility?.Name ?? "null");
                
                if (facility == null)
                {
                    _logger.LogWarning("Facility data is null");
                    return BadRequest("Facility data is required");
                }

                var result = await _facilityRepository.Insert(facility);
                if (result == null)
                {
                    _logger.LogError("Failed to create facility - repository returned null");
                    return BadRequest("Failed to create facility");
                }

                _logger.LogInformation("Successfully created facility with ID: {Id}", result.Id);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating facility");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FacilityEntity>> Update(int id, [FromBody] FacilityEntity facility)
        {
            try
            {
                _logger.LogInformation("Updating facility with ID: {Id}", id);
                
                if (facility == null)
                {
                    _logger.LogWarning("Facility data is null");
                    return BadRequest("Facility data is required");
                }

                if (id != facility.Id)
                {
                    _logger.LogWarning("ID mismatch - URL ID: {UrlId}, Body ID: {BodyId}", id, facility.Id);
                    return BadRequest("ID mismatch");
                }

                var result = await _facilityRepository.Update(facility);
                if (result == null)
                {
                    _logger.LogWarning("Facility not found or update failed for ID: {Id}", id);
                    return NotFound("Facility not found or update failed");
                }

                _logger.LogInformation("Successfully updated facility with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating facility with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                _logger.LogInformation("Deleting facility with ID: {Id}", id);
                
                var facility = await _facilityRepository.GetById(id);
                if (facility == null)
                {
                    _logger.LogWarning("Facility not found for deletion - ID: {Id}", id);
                    return NotFound();
                }

                var deleted = await _facilityRepository.DeleteById(id);
                if (!deleted)
                {
                    _logger.LogError("Failed to delete facility with ID: {Id}", id);
                    return StatusCode(500, "Failed to delete facility");
                }
                
                _logger.LogInformation("Successfully deleted facility with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting facility with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}