namespace Workforce.Server.Models
{
    public class ScheduleExportRequest
    {
        public List<ScheduleAppointmentDto> Appointments { get; set; } = new();
        public string Title { get; set; } = "Visualização da Escala";
        public string Subtitle { get; set; } = "";
    }

    public class ScheduleAppointmentDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string HumanResourceName { get; set; } = string.Empty;
        public string WorkUnitName { get; set; } = string.Empty;
        public string JobTitleName { get; set; } = string.Empty;
        public string PeriodName { get; set; } = string.Empty;
    }
}
