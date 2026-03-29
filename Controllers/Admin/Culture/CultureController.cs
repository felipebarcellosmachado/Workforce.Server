using Microsoft.AspNetCore.Mvc;
using Workforce.Domain.Admin.Culture.Entity;
using Workforce.Realization.Infrastructure.Persistence.Admin.Culture;

namespace Workforce.Server.Controllers.Admin.Culture
{
    [ApiController]
    [Route("api/admin/culture")]
    public class CultureController : ControllerBase
    {
        private readonly CultureRepository _cultureRepository;
        private readonly ILogger<CultureController> _logger;

        public CultureController(CultureRepository cultureRepository, ILogger<CultureController> logger)
        {
            _cultureRepository = cultureRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Admin.Culture.Entity.Culture>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting culture by ID: {Id}", id);
                var culture = await _cultureRepository.GetById(id);
                if (culture == null)
                {
                    _logger.LogWarning("Culture not found for ID: {Id}", id);
                    return NotFound();
                }
                _logger.LogInformation("Successfully retrieved culture with ID: {Id}", id);
                return Ok(culture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting culture by ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Domain.Admin.Culture.Entity.Culture>> GetByCode(string code)
        {
            try
            {
                _logger.LogInformation("Getting culture by code: {Code}", code);
                var culture = await _cultureRepository.GetByCode(code);
                if (culture == null)
                {
                    _logger.LogWarning("Culture not found for code: {Code}", code);
                    return NotFound();
                }
                _logger.LogInformation("Successfully retrieved culture with code: {Code}", code);
                return Ok(culture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting culture by code: {Code}", code);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<Domain.Admin.Culture.Entity.Culture>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all cultures");
                var cultures = await _cultureRepository.GetAll();
                _logger.LogInformation("Successfully retrieved {Count} cultures", cultures.Count);
                return Ok(cultures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cultures");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IList<Domain.Admin.Culture.Entity.Culture>>> GetAllActive()
        {
            try
            {
                _logger.LogInformation("Getting all active cultures");
                var cultures = await _cultureRepository.GetAllActive();
                _logger.LogInformation("Successfully retrieved {Count} active cultures", cultures.Count);
                return Ok(cultures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active cultures");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("default")]
        public async Task<ActionResult<Domain.Admin.Culture.Entity.Culture>> GetDefault()
        {
            try
            {
                _logger.LogInformation("Getting default culture");
                var culture = await _cultureRepository.GetDefault();
                if (culture == null)
                {
                    _logger.LogWarning("No default culture found");
                    return NotFound("No default culture configured");
                }
                _logger.LogInformation("Successfully retrieved default culture: {Code}", culture.Code);
                return Ok(culture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default culture");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Admin.Culture.Entity.Culture>> Insert([FromBody] Domain.Admin.Culture.Entity.Culture culture)
        {
            try
            {
                if (culture == null)
                {
                    return BadRequest("Culture data is required");
                }

                if (string.IsNullOrWhiteSpace(culture.Code))
                {
                    return BadRequest("Culture code is required");
                }

                if (string.IsNullOrWhiteSpace(culture.Name))
                {
                    return BadRequest("Culture name is required");
                }

                _logger.LogInformation("Creating new culture with code: {Code}", culture.Code);

                var result = await _cultureRepository.Insert(culture);
                if (result == null)
                {
                    _logger.LogWarning("Failed to create culture with code: {Code}", culture.Code);
                    return BadRequest("Failed to create culture. Code may already exist.");
                }

                _logger.LogInformation("Successfully created culture with ID: {Id} and code: {Code}", result.Id, result.Code);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating culture");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Admin.Culture.Entity.Culture>> Update(int id, [FromBody] Domain.Admin.Culture.Entity.Culture culture)
        {
            try
            {
                if (culture == null)
                {
                    return BadRequest("Culture data is required");
                }

                if (id != culture.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (string.IsNullOrWhiteSpace(culture.Code))
                {
                    return BadRequest("Culture code is required");
                }

                if (string.IsNullOrWhiteSpace(culture.Name))
                {
                    return BadRequest("Culture name is required");
                }

                _logger.LogInformation("Updating culture with ID: {Id}", id);

                var result = await _cultureRepository.Update(culture);
                if (result == null)
                {
                    _logger.LogWarning("Failed to update culture with ID: {Id}", id);
                    return NotFound("Culture not found or update failed");
                }

                _logger.LogInformation("Successfully updated culture with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating culture with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                _logger.LogInformation("Deleting culture with ID: {Id}", id);

                var success = await _cultureRepository.DeleteById(id);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete culture with ID: {Id}", id);
                    return NotFound("Culture not found or cannot delete default culture");
                }

                _logger.LogInformation("Successfully deleted culture with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting culture with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}/resources")]
        public async Task<ActionResult<Domain.Admin.Culture.Entity.Culture>> UpdateResources(int id, [FromBody] Dictionary<string, string> resources)
        {
            try
            {
                if (resources == null)
                {
                    return BadRequest("Resources data is required");
                }

                _logger.LogInformation("Updating resources for culture with ID: {Id}", id);

                var result = await _cultureRepository.UpdateResources(id, resources);
                if (result == null)
                {
                    _logger.LogWarning("Failed to update resources for culture with ID: {Id}", id);
                    return NotFound("Culture not found");
                }

                _logger.LogInformation("Successfully updated {Count} resources for culture with ID: {Id}", resources.Count, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resources for culture with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/resources")]
        public async Task<ActionResult<Dictionary<string, string>>> GetResources(int id)
        {
            try
            {
                _logger.LogInformation("Getting resources for culture with ID: {Id}", id);

                var culture = await _cultureRepository.GetById(id);
                if (culture == null)
                {
                    _logger.LogWarning("Culture not found for ID: {Id}", id);
                    return NotFound("Culture not found");
                }

                var resources = culture.GetResources();
                _logger.LogInformation("Successfully retrieved {Count} resources for culture with ID: {Id}", resources.Count, id);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resources for culture with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}