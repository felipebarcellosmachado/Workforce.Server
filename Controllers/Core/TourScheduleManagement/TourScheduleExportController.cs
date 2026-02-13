using Microsoft.AspNetCore.Mvc;
using Workforce.Server.Models;
using Workforce.Server.Services;

namespace Workforce.Server.Controllers.Core.TourScheduleManagement
{
    [ApiController]
    [Route("api/core/tour-schedule-management/export")]
    public class TourScheduleExportController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly ILogger<TourScheduleExportController> _logger;

        public TourScheduleExportController(
            IExportService exportService,
            ILogger<TourScheduleExportController> logger)
        {
            _exportService = exportService;
            _logger = logger;
        }

        [HttpPost("csv")]
        public IActionResult ExportToCsv([FromBody] ScheduleExportRequest request)
        {
            try
            {
                var headers = new[] 
                { 
                    "Data Início", 
                    "Hora Início", 
                    "Data Fim", 
                    "Hora Fim", 
                    "Recurso", 
                    "Unidade de Trabalho", 
                    "Cargo", 
                    "Período", 
                    "Duração (horas)" 
                };

                var csvBytes = _exportService.ExportToCsv(
                    request.Appointments.OrderBy(a => a.Start),
                    a => new object[] 
                    {
                        a.Start.ToString("dd/MM/yyyy"),
                        a.Start.ToString("HH:mm"),
                        a.End.ToString("dd/MM/yyyy"),
                        a.End.ToString("HH:mm"),
                        a.HumanResourceName,
                        a.WorkUnitName,
                        a.JobTitleName,
                        a.PeriodName,
                        (a.End - a.Start).TotalHours.ToString("N2")
                    },
                    headers
                );

                return File(csvBytes, "text/csv", $"Escala_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to CSV");
                return StatusCode(500, new { error = "Erro ao exportar CSV", message = ex.Message });
            }
        }

        [HttpPost("excel")]
        public IActionResult ExportToExcel([FromBody] ScheduleExportRequest request)
        {
            try
            {
                var excelBytes = _exportService.ExportScheduleGridToExcel(
                    request.Appointments.OrderBy(a => a.Start).ToList(),
                    request.Title,
                    request.Subtitle
                );

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Escala_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                return StatusCode(500, new { error = "Erro ao exportar Excel", message = ex.Message });
            }
        }

        [HttpPost("pdf")]
        public IActionResult ExportToPdf([FromBody] ScheduleExportRequest request)
        {
            try
            {
                var pdfBytes = _exportService.ExportScheduleGridToPdf(
                    request.Appointments.OrderBy(a => a.Start).ToList(),
                    request.Title,
                    request.Subtitle
                );

                return File(pdfBytes, "application/pdf", $"Escala_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to PDF");
                return StatusCode(500, new { error = "Erro ao exportar PDF", message = ex.Message });
            }
        }
    }
}
