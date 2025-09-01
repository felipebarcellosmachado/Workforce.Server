using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Core.HumanResourceManagement.Qualification.Repository;

namespace Workforce.Server.Controllers.Core.HumanResourceManagement.Qualification
{
    [ApiController]
    [Route("api/core/human_resource/[controller]")]
    public class QualificationController : ControllerBase
    {
        private readonly QualificationRepository _qualificationRepository;

        public QualificationController(QualificationRepository qualificationRepository)
        {
            _qualificationRepository = qualificationRepository;
        }

        // HttpGet para GetById
        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification>> GetById(int id)
        {
            try
            {
                var qualification = await _qualificationRepository.GetById(id);
                if (qualification == null)
                {
                    return NotFound($"Qualification com ID {id} não encontrado");
                }
                return Ok(qualification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpGet("environment/{id}/{id}") para GetByEnvironmentIdAndId 
        [HttpGet("environment/{environmentId}/{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification>> GetByEnvironmentIdAndId(int environmentId, int id)
        {
            try
            {
                var qualification = await _qualificationRepository.GetById(id);
                if (qualification == null || qualification.EnvironmentId != environmentId)
                {
                    return NotFound($"Qualification com ID {id} não encontrado para o Environment {environmentId}");
                }
                return Ok(qualification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpGet("all/environment/{id}") para GetAllByEnvironmentId
        [HttpGet("all/environment/{environmentId}")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var qualifications = await _qualificationRepository.GetAllByEnvironmentId(environmentId);
                return Ok(qualifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpGet para GetAll
        [HttpGet("all")]
        public async Task<ActionResult<IList<Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification>>> GetAll()
        {
            try
            {
                var qualifications = await _qualificationRepository.GetAll();
                return Ok(qualifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpPut para Update
        [HttpPut("{id}")]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification>> Update(int id, [FromBody] Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification qualification)
        {
            try
            {
                if (id != qualification.Id)
                {
                    return BadRequest("ID da URL não corresponde ao ID do objeto");
                }

                var updatedQualification = await _qualificationRepository.Update(qualification);
                return Ok(updatedQualification);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpPost para Insert
        [HttpPost]
        public async Task<ActionResult<Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification>> Insert([FromBody] Domain.Core.HumanResourceManagement.Qualification.Entity.Qualification qualification)
        {
            try
            {
                var insertedQualification = await _qualificationRepository.Insert(qualification);
                return CreatedAtAction(nameof(GetById), new { id = insertedQualification.Id }, insertedQualification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // HttpDelete para DeleteById
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                var deleted = await _qualificationRepository.DeleteById(id);
                if (!deleted)
                {
                    return NotFound($"Qualification com ID {id} não encontrado");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}