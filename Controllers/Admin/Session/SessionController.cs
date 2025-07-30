using Microsoft.AspNetCore.Mvc;
using Workforce.Business.Admin.Session.Repository;
using Workforce.Business.Admin.Session.Dto;
using Workforce.Domain.Admin.Session.Entity;
using Workforce.Domain.Infra.Role.Entity;

namespace Workforce.Server.Controllers.Admin.Session
{
    [ApiController]
    [Route("api/admin/session/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly SessionRepository _sessionRepository;
        private readonly ILogger<SessionController> _logger;

        public SessionController(SessionRepository sessionRepository, ILogger<SessionController> logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get session by ID
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <returns>Session if found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Admin.Session.Entity.Session>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting session by ID: {Id}", id);
                var session = await _sessionRepository.GetById(id);
                if (session == null)
                {
                    _logger.LogWarning("Session not found for ID: {Id}", id);
                    return NotFound();
                }
                _logger.LogInformation("Successfully retrieved session with ID: {Id}", id);
                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session by ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all sessions ordered by login date (descending)
        /// </summary>
        /// <returns>List of all sessions</returns>
        [HttpGet]
        public async Task<ActionResult<List<Domain.Admin.Session.Entity.Session>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all sessions");
                var sessions = await _sessionRepository.GetAll();
                _logger.LogInformation("Successfully retrieved {Count} sessions", sessions.Count);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sessions");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all sessions for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of sessions for the user</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Domain.Admin.Session.Entity.Session>>> GetAllByUserId(int userId)
        {
            try
            {
                _logger.LogInformation("Getting sessions for user ID: {UserId}", userId);
                var sessions = await _sessionRepository.GetAllByUserId(userId);
                _logger.LogInformation("Successfully retrieved {Count} sessions for user {UserId}", sessions.Count, userId);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for user ID: {UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new login session
        /// </summary>
        /// <param name="loginRequest">Login request containing user and IP information</param>
        /// <returns>Created session</returns>
        [HttpPost("login")]
        public async Task<ActionResult<Domain.Admin.Session.Entity.Session>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var session = await _sessionRepository.Login(loginRequest);

                if (session == null)
                {
                    _logger.LogError("Failed to create login session for login: {Login}", loginRequest.Login);
                    return BadRequest("Failed to create login session");
                }

                _logger.LogInformation("Successfully created login session with ID: {SessionId} for login: {Login}", 
                    session.Id, loginRequest.Login);
                return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating login session");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Logout from a session
        /// </summary>
        /// <param name="sessionId">Session ID to logout</param>
        /// <returns>Updated session with logout time</returns>
        [HttpPost("logout/{sessionId}")]
        public async Task<ActionResult<Domain.Admin.Session.Entity.Session>> Logout(int sessionId)
        {
            try
            {
                _logger.LogInformation("Logging out session ID: {SessionId}", sessionId);

                var existingSession = await _sessionRepository.GetById(sessionId);
                if (existingSession == null)
                {
                    _logger.LogWarning("Session not found for logout: {SessionId}", sessionId);
                    return NotFound("Session not found");
                }

                var session = await _sessionRepository.Logout(existingSession);
                _logger.LogInformation("Successfully logged out session ID: {SessionId}", sessionId);
                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging out session ID: {SessionId}", sessionId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update session activity (heartbeat)
        /// </summary>
        /// <param name="sessionId">Session ID to update</param>
        /// <returns>Updated session with current activity time</returns>
        [HttpPost("heartbeat/{sessionId}")]
        public async Task<ActionResult<Domain.Admin.Session.Entity.Session>> HeartBeat(int sessionId)
        {
            try
            {
                _logger.LogInformation("Updating heartbeat for session ID: {SessionId}", sessionId);

                var existingSession = await _sessionRepository.GetById(sessionId);
                if (existingSession == null)
                {
                    _logger.LogWarning("Session not found for heartbeat: {SessionId}", sessionId);
                    return NotFound("Session not found");
                }

                var session = await _sessionRepository.HeartBeat(existingSession);
                _logger.LogInformation("Successfully updated heartbeat for session ID: {SessionId}", sessionId);
                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating heartbeat for session ID: {SessionId}", sessionId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get last N login sessions
        /// </summary>
        /// <param name="count">Number of recent logins to retrieve</param>
        /// <returns>List of recent login sessions</returns>
        [HttpGet("last-logins/{count}")]
        public async Task<ActionResult<List<Domain.Admin.Session.Entity.Session>>> LastLogins(int count)
        {
            try
            {
                _logger.LogInformation("Getting last {Count} login sessions", count);

                if (count <= 0)
                {
                    return BadRequest("Count must be greater than 0");
                }

                if (count > 1000) // Reasonable limit
                {
                    return BadRequest("Count cannot exceed 1000");
                }

                var sessions = await _sessionRepository.LastLogins(count);
                _logger.LogInformation("Successfully retrieved {Count} recent login sessions", sessions.Count);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last {Count} login sessions", count);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get sessions with activity within the last N seconds
        /// </summary>
        /// <param name="seconds">Number of seconds to look back for activity</param>
        /// <returns>List of recently active sessions</returns>
        [HttpGet("last-activities/{seconds}")]
        public async Task<ActionResult<List<Domain.Admin.Session.Entity.Session>>> LastActivities(int seconds)
        {
            try
            {
                _logger.LogInformation("Getting sessions with activity in the last {Seconds} seconds", seconds);

                if (seconds <= 0)
                {
                    return BadRequest("Seconds must be greater than 0");
                }

                if (seconds > 86400) // 24 hours limit
                {
                    return BadRequest("Seconds cannot exceed 86400 (24 hours)");
                }

                var sessions = await _sessionRepository.LastActivities(seconds);
                _logger.LogInformation("Successfully retrieved {Count} recently active sessions", sessions.Count);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions with activity in last {Seconds} seconds", seconds);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}