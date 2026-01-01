using Microsoft.AspNetCore.Mvc;
using Workforce.Domain.Infra.Party.Entity;
using Workforce.Realization.Infrastructure.Persistence.Infra.Party.Organization;

namespace Workforce.Server.Controllers.Infra.Party
{
    [ApiController]
    [Route("api/infra/party/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationRepository _organizationRepository;

        public OrganizationController(OrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetById(int id)
        {
            try
            {
                var organization = await _organizationRepository.GetById(id);
                if (organization == null)
                {
                    return NotFound();
                }
                return Ok(organization);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<Organization>> GetByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Organization name is required");
                }

                var organization = await _organizationRepository.GetByName(name);
                if (organization == null)
                {
                    return NotFound($"Organization with name '{name}' not found");
                }
                return Ok(organization);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/name/{name}")]
        public async Task<ActionResult<Organization>> GetByEnvironmentIdAndName(int environmentId, string name)
        {
            try
            {
                if (environmentId <= 0)
                {
                    return BadRequest("Environment ID must be greater than zero");
                }
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Organization name is required");
                }

                var organization = await _organizationRepository.GetByEnvironmentIdAndName(environmentId, name);
                if (organization == null)
                {
                    return NotFound($"Organization with name '{name}' in environment '{environmentId}' not found");
                }
                return Ok(organization);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Organization>>> GetAll()
        {
            try
            {
                var organizations = await _organizationRepository.GetAll();
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}")]
        public async Task<ActionResult<List<Organization>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var organizations = await _organizationRepository.GetAllByEnvironmentId(environmentId);
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Organization>> Insert([FromBody] Organization organization)
        {
            try
            {
                if (organization == null)
                {
                    return BadRequest("Organization data is required");
                }

                var result = await _organizationRepository.Insert(organization);
                if (result == null)
                {
                    return BadRequest("Failed to create organization");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Organization>> Update(int id, [FromBody] Organization organization)
        {
            try
            {
                if (organization == null)
                {
                    return BadRequest("Organization data is required");
                }

                if (id != organization.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _organizationRepository.Update(organization);
                if (result == null)
                {
                    return NotFound("Organization not found or update failed");
                }

                return Ok(result);
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
                var organization = await _organizationRepository.GetById(id);
                if (organization == null)
                {
                    return NotFound();
                }

                var deleted = await _organizationRepository.DeleteById(id);
                if (!deleted)
                {
                    return StatusCode(500, "Failed to delete organization");
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