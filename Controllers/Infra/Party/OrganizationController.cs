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
        public async Task<ActionResult<Organization>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(id, ct);
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
        public async Task<ActionResult<Organization>> GetByName(string name, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Organization name is required");
                }

                var organization = await _organizationRepository.GetByNameAsync(name, ct);
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
        public async Task<ActionResult<Organization>> GetByEnvironmentIdAndName(int environmentId, string name, CancellationToken ct = default)
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

                var organization = await _organizationRepository.GetByEnvironmentIdAndNameAsync(environmentId, name, ct);
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
        public async Task<ActionResult<List<Organization>>> GetAll(CancellationToken ct = default)
        {
            try
            {
                var organizations = await _organizationRepository.GetAllAsync(ct);
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}")]
        public async Task<ActionResult<List<Organization>>> GetAllByEnvironmentId(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var organizations = await _organizationRepository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Organization>> Insert([FromBody] Organization organization, CancellationToken ct = default)
        {
            try
            {
                if (organization == null)
                {
                    return BadRequest("Organization data is required");
                }

                var result = await _organizationRepository.InsertAsync(organization, ct);
                if (result == null)
                {
                    return BadRequest("Failed to create organization");
                }

                return Created($"api/infra/party/organization/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Organization>> Update(int id, [FromBody] Organization organization, CancellationToken ct = default)
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

                var result = await _organizationRepository.UpdateAsync(organization, ct);
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
        public async Task<ActionResult> DeleteById(int id, CancellationToken ct = default)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(id, ct);
                if (organization == null)
                {
                    return NotFound();
                }

                var deleted = await _organizationRepository.DeleteByIdAsync(id, ct);
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