using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.Party.Organization;
using Workforce.Domain.Infra.Party.Entity;

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