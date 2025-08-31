using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Infra.PartyRole.Repository;
using Workforce.Domain.Infra.Role.User.Entity;

namespace Workforce.Server.Controllers.Infra.Role
{
    [ApiController]
    [Route("api/infra/role/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(UserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting user by ID: {Id}", id);
                var user = await _userRepository.GetById(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {Id}", id);
                    return NotFound();
                }
                _logger.LogInformation("Successfully retrieved user with ID: {Id}", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("login/{login}")]
        public async Task<ActionResult<User>> GetByLogin(string login)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login))
                {
                    return BadRequest("User login is required");
                }

                _logger.LogInformation("Getting user by login: {Login}", login);
                var user = await _userRepository.GetByLogin(login);
                if (user == null)
                {
                    _logger.LogWarning("User not found for login: {Login}", login);
                    return NotFound($"User with login '{login}' not found");
                }
                _logger.LogInformation("Successfully retrieved user with login: {Login}", login);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by login: {Login}", login);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("environment/{environmentId}/login/{login}")]
        public async Task<ActionResult<User>> GetByEnvironmentIdAndLogin(int environmentId, string login)
        {
            try
            {
                if (environmentId <= 0)
                {
                    return BadRequest("Environment ID must be greater than zero");
                }
                if (string.IsNullOrWhiteSpace(login))
                {
                    return BadRequest("User login is required");
                }

                _logger.LogInformation("Getting user by environment ID: {EnvironmentId} and login: {Login}", environmentId, login);
                var user = await _userRepository.GetByEnvironmentIdAndLogin(environmentId, login);
                if (user == null)
                {
                    _logger.LogWarning("User not found for environment ID: {EnvironmentId} and login: {Login}", environmentId, login);
                    return NotFound($"User with login '{login}' in environment '{environmentId}' not found");
                }
                _logger.LogInformation("Successfully retrieved user with environment ID: {EnvironmentId} and login: {Login}", environmentId, login);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by environment ID: {EnvironmentId} and login: {Login}", environmentId, login);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all users");
                var users = await _userRepository.GetAll();
                _logger.LogInformation("Successfully retrieved {Count} users", users.Count);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> Insert([FromBody] User user)
        {
            try
            {
                _logger.LogInformation("Creating new user with login: {Login}", user?.Login ?? "null");
                
                if (user == null)
                {
                    _logger.LogWarning("User data is null");
                    return BadRequest("User data is required");
                }

                // Log more details about the user being created
                _logger.LogInformation("User details - Login: {Login}, PersonId: {PersonId}, Person: {HasPerson}", 
                    user.Login, user.PersonId, user.Person != null ? "Yes" : "No");

                var result = await _userRepository.Insert(user);
                if (result == null)
                {
                    _logger.LogError("Failed to create user - repository returned null");
                    return BadRequest("Failed to create user");
                }

                _logger.LogInformation("Successfully created user with ID: {Id}", result.Id);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] User user)
        {
            try
            {
                _logger.LogInformation("Updating user with ID: {Id}", id);
                
                if (user == null)
                {
                    _logger.LogWarning("User data is null for update");
                    return BadRequest("User data is required");
                }

                if (id != user.Id)
                {
                    _logger.LogWarning("ID mismatch - URL ID: {UrlId}, User ID: {UserId}", id, user.Id);
                    return BadRequest("ID mismatch");
                }

                var result = await _userRepository.Update(user);
                if (result == null)
                {
                    _logger.LogWarning("User not found or update failed for ID: {Id}", id);
                    return NotFound("User not found or update failed");
                }

                _logger.LogInformation("Successfully updated user with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID: {Id}", id);
                await _userRepository.DeleteById(id);
                _logger.LogInformation("Successfully deleted user with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}