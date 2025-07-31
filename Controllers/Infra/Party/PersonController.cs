using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.Party.Person;
using Workforce.Domain.Infra.Party.Entity;

namespace Workforce.Server.Controllers.Infra.Party
{
    [ApiController]
    [Route("api/infra/party/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly PersonRepository _personRepository;

        public PersonController(PersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id)
        {
            try
            {
                var person = await _personRepository.GetById(id);
                if (person == null)
                {
                    return NotFound();
                }
                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Person>>> GetAll()
        {
            try
            {
                var persons = await _personRepository.GetAll();
                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}")]
        public async Task<ActionResult<List<Person>>> GetAllByEnvironmentId(int environmentId)
        {
            try
            {
                var persons = await _personRepository.GetAllByEnvironmentId(environmentId);
                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Person>> Insert([FromBody] Person person)
        {
            try
            {
                if (person == null)
                {
                    return BadRequest("Person data is required");
                }

                var result = await _personRepository.Insert(person);
                if (result == null)
                {
                    return BadRequest("Failed to create person");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> Update(int id, [FromBody] Person person)
        {
            try
            {
                if (person == null)
                {
                    return BadRequest("Person data is required");
                }

                if (id != person.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _personRepository.Update(person);
                if (result == null)
                {
                    return NotFound("Person not found or update failed");
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
                await _personRepository.DeleteById(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
