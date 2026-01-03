using Microsoft.AspNetCore.Mvc;
using Workforce.Realization.Infrastructure.External.Scripts;
using Workforce.Realization.Infrastructure.Persistence.Infra.Party.Person;

namespace Workforce.Server.Controllers.Infra.Party
{
    /// <summary>
    /// Controller temporário para execução de scripts de dados
    /// IMPORTANTE: Remover ou proteger após uso em produção
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class DataScriptsController : ControllerBase
    {
        private readonly PersonRepository _personRepository;
        private readonly ILogger<DataScriptsController> _logger;

        public DataScriptsController(PersonRepository personRepository, ILogger<DataScriptsController> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        /// <summary>
        /// Executa o script de inserção de pessoas (agentes)
        /// POST /api/admin/datascripts/insert-persons
        /// </summary>
        [HttpPost("insert-persons")]
        public async Task<IActionResult> InsertPersons()
        {
            try
            {
                var script = new InsertPersonsScript(_personRepository);
                var count = await script.ExecuteAsync();

                _logger.LogInformation("Script de inserção de pessoas executado com sucesso. {Count} pessoas inseridas.", count);

                return Ok(new
                {
                    success = true,
                    message = $"{count} pessoas inseridas com sucesso",
                    count = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar script de inserção de pessoas");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao inserir pessoas",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Verifica quantas pessoas existem no banco
        /// GET /api/admin/datascripts/count-persons
        /// </summary>
        [HttpGet("count-persons")]
        public async Task<IActionResult> CountPersons()
        {
            try
            {
                var persons = await _personRepository.GetAll();
                var personCount = persons.Count;

                return Ok(new
                {
                    persons = personCount,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar pessoas");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
