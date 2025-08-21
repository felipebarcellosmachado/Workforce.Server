using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.HumanResourceManagement.WorkAgreement.Repository;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.WorkAgreement
{
    [ApiController]
    [Route("api/infra/[controller]")]
    public class WorkAgreementController : ControllerBase
    {
        private readonly WorkAgreementRepository _workAgreementRepository;

        public WorkAgreementController(WorkAgreementRepository workAgreementRepository)
        {
            _workAgreementRepository = workAgreementRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.WorkAgreement.Entity.WorkAgreement>> GetById(int id)
        {
            try
            {
                var workAgreement = await _workAgreementRepository.GetById(id);
                if (workAgreement == null)
                {
                    return NotFound();
                }
                return Ok(workAgreement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.WorkAgreement.Entity.WorkAgreement>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var workAgreement = await _workAgreementRepository.GetById(id);
                if (workAgreement == null || workAgreement.EnvironmentId != environmentId)
                {
                    return NotFound();
                }
                return Ok(workAgreement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.WorkAgreement.Entity.WorkAgreement>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var workAgreements = await _workAgreementRepository.GetAllByEnvironmentId(environmentId);
                return Ok(workAgreements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.WorkAgreement.Entity.WorkAgreement>> Insert([FromBody] Domain.Core.HumanResourceManagement.WorkAgreement.Entity.WorkAgreement workAgreement)
        {
            try
            {
                if (workAgreement == null)
                {
                    return BadRequest("WorkAgreement data is required");
                }

                var result = await _workAgreementRepository.Insert(workAgreement);
                if (result == null)
                {
                    return BadRequest("Failed to create work agreement");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.WorkAgreement.Entity.WorkAgreement>> Update(int id, [FromBody] Domain.Core.HumanResourceManagement.WorkAgreement.Entity.WorkAgreement workAgreement)
        {
            try
            {
                if (workAgreement == null)
                {
                    return BadRequest("WorkAgreement data is required");
                }

                if (id != workAgreement.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _workAgreementRepository.Update(workAgreement);
                if (result == null)
                {
                    return NotFound("WorkAgreement not found or update failed");
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
                var success = await _workAgreementRepository.DeleteById(id);
                if (!success)
                {
                    return NotFound("WorkAgreement not found");
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