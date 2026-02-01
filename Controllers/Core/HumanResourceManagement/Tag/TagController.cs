using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.Persistence.Core.HumanResourceManagement.Tag;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.Tag
{
    [ApiController]
    [Route("api/core/human_resource_management/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly TagRepository tagRepository;

        public TagController(TagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Tag.Entity.Tag>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var tag = await tagRepository.GetByIdAsync(id, ct);
                if (tag == null)
                {
                    return NotFound();
                }
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Tag.Entity.Tag>>> GetAllByEnvironmentIdAsync(int environmentId, CancellationToken ct = default)
        {
            try
            {
                var tags = await tagRepository.GetAllByEnvironmentIdAsync(environmentId, ct);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Tag.Entity.Tag>> InsertAsync([FromBody] Domain.Core.HumanResourceManagement.Tag.Entity.Tag tag, CancellationToken ct = default)
        {
            try
            {
                if (tag == null)
                {
                    return BadRequest("Tag data is required");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdTag = await tagRepository.InsertAsync(tag, ct);
                return Created($"/api/core/human_resource_management/tag/{createdTag.Id}", createdTag);
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
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Tag.Entity.Tag>> UpdateAsync(int id, [FromBody] Domain.Core.HumanResourceManagement.Tag.Entity.Tag tag, CancellationToken ct = default)
        {
            try
            {
                if (tag == null)
                {
                    return BadRequest("Tag data is required");
                }

                if (id != tag.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedTag = await tagRepository.UpdateAsync(tag, ct);
                return Ok(updatedTag);
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
        public async Task<ActionResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            try
            {
                await tagRepository.DeleteAsync(id, ct);
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
