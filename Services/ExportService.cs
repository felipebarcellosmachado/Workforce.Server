using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;
using Workforce.Server.Models;

namespace Workforce.Server.Services
{
    public interface IExportService
    {
        byte[] ExportToCsv<T>(IEnumerable<T> data, Func<T, object[]> rowMapper, string[] headers);
        byte[] ExportToExcel<T>(IEnumerable<T> data, Func<T, object[]> rowMapper, string[] headers, string sheetName);
        byte[] ExportToPdf<T>(IEnumerable<T> data, Func<T, object[]> rowMapper, string[] headers, string title, string subtitle = "");

        /// <summary>Generates an Excel workbook with a schedule grid (Resources × Dates) similar to the HeatMap view.</summary>
        byte[] ExportScheduleGridToExcel(List<ScheduleAppointmentDto> appointments, string title, string subtitle);

        /// <summary>Generates a PDF document with a schedule grid (Resources × Dates) similar to the HeatMap view.</summary>
        byte[] ExportScheduleGridToPdf(List<ScheduleAppointmentDto> appointments, string title, string subtitle);
    }

    public class ExportService : IExportService
    {
        public byte[] ExportToCsv<T>(IEnumerable<T> data, Func<T, object[]> rowMapper, string[] headers)
        {
            var csv = new StringBuilder();
            
            // Add headers
            csv.AppendLine(string.Join(",", headers.Select(h => EscapeCsvValue(h))));
            
            // Add data rows
            foreach (var item in data)
            {
                var values = rowMapper(item);
                csv.AppendLine(string.Join(",", values.Select(v => EscapeCsvValue(v?.ToString() ?? ""))));
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public byte[] ExportToExcel<T>(IEnumerable<T> data, Func<T, object[]> rowMapper, string[] headers, string sheetName)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);
            
            // Add headers
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }
            
            // Add data rows
            int row = 2;
            foreach (var item in data)
            {
                var values = rowMapper(item);
                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    if (value != null)
                    {
                        // Handle different types appropriately
                        if (value is DateTime dateTime)
                        {
                            worksheet.Cell(row, i + 1).Value = dateTime;
                            worksheet.Cell(row, i + 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                        }
                        else if (value is double || value is decimal || value is float)
                        {
                            worksheet.Cell(row, i + 1).Value = Convert.ToDouble(value);
                            worksheet.Cell(row, i + 1).Style.NumberFormat.Format = "#,##0.00";
                        }
                        else if (value is int || value is long)
                        {
                            worksheet.Cell(row, i + 1).Value = Convert.ToInt64(value);
                        }
                        else
                        {
                            worksheet.Cell(row, i + 1).Value = value.ToString();
                        }
                    }
                }
                row++;
            }
            
            // Auto-fit columns
            worksheet.Columns().AdjustToContents();
            
            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportToPdf<T>(IEnumerable<T> data, Func<T, object[]> rowMapper, string[] headers, string title, string subtitle = "")
        {
            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    
                    page.Header().Column(column =>
                    {
                        column.Item().Text(title).FontSize(20).Bold();
                        if (!string.IsNullOrEmpty(subtitle))
                        {
                            column.Item().Text(subtitle).FontSize(12);
                        }
                        column.Item().PaddingVertical(5).LineHorizontal(1);
                    });
                    
                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        // Define columns
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in headers)
                            {
                                columns.RelativeColumn();
                            }
                        });
                        
                        // Add header
                        table.Header(header =>
                        {
                            foreach (var headerText in headers)
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text(headerText).Bold();
                            }
                        });
                        
                        // Add data rows
                        foreach (var item in data)
                        {
                            var values = rowMapper(item);
                            foreach (var value in values)
                            {
                                table.Cell().Padding(5).Text(FormatPdfValue(value));
                            }
                        }
                    });
                    
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            });
            
            return document.GeneratePdf();
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Escape quotes and wrap in quotes if contains comma, quote, or newline
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        private string FormatPdfValue(object? value)
        {
            if (value == null) return "";

            if (value is DateTime dateTime)
                return dateTime.ToString("dd/MM/yyyy HH:mm");

            if (value is double || value is decimal || value is float)
                return Convert.ToDouble(value).ToString("N2");

            return value.ToString() ?? "";
        }

        // ── Schedule-grid helpers ───────────────────────────────────────

        private record GridCell(double Hours, int Count, string Period, string WorkUnit);

        private static (List<string> Resources, List<DateTime> Dates, Dictionary<(string, DateTime), GridCell> Cells)
            BuildGrid(List<ScheduleAppointmentDto> appointments)
        {
            var cells = appointments
                .GroupBy(a => (a.HumanResourceName, a.Start.Date))
                .ToDictionary(
                    g => (g.Key.HumanResourceName, g.Key.Date),
                    g => new GridCell(
                        g.Sum(a => (a.End - a.Start).TotalHours),
                        g.Count(),
                        string.Join(", ", g.Select(a => a.PeriodName).Where(p => !string.IsNullOrEmpty(p)).Distinct()),
                        string.Join(", ", g.Select(a => a.WorkUnitName).Where(w => !string.IsNullOrEmpty(w)).Distinct())
                    ));

            var resources = appointments.Select(a => a.HumanResourceName).Distinct().OrderBy(r => r).ToList();
            var dates = appointments.Select(a => a.Start.Date).Distinct().OrderBy(d => d).ToList();

            return (resources, dates, cells);
        }

        private static readonly string[] DayNames = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"];

        // ── Excel grid export ───────────────────────────────────────────

        public byte[] ExportScheduleGridToExcel(List<ScheduleAppointmentDto> appointments, string title, string subtitle)
        {
            var (resources, dates, cells) = BuildGrid(appointments);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Escala");

            // Title
            int row = 1;
            ws.Cell(row, 1).Value = title;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 16;
            ws.Range(row, 1, row, dates.Count + 1).Merge();
            row++;

            if (!string.IsNullOrEmpty(subtitle))
            {
                ws.Cell(row, 1).Value = subtitle;
                ws.Cell(row, 1).Style.Font.FontSize = 11;
                ws.Cell(row, 1).Style.Font.Italic = true;
                ws.Range(row, 1, row, dates.Count + 1).Merge();
                row++;
            }
            row++; // blank row

            // Header row: "Recurso" + dates
            ws.Cell(row, 1).Value = "Recurso";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.DarkBlue;
            ws.Cell(row, 1).Style.Font.FontColor = XLColor.White;
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int c = 0; c < dates.Count; c++)
            {
                var cell = ws.Cell(row, c + 2);
                cell.Value = $"{dates[c]:dd/MM}\n{DayNames[(int)dates[c].DayOfWeek]}";
                cell.Style.Alignment.WrapText = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Font.Bold = true;

                var isWeekend = dates[c].DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
                cell.Style.Fill.BackgroundColor = isWeekend ? XLColor.FromArgb(0x37474F) : XLColor.FromArgb(0x1565C0);
                cell.Style.Font.FontColor = XLColor.White;
            }
            row++;

            // Data rows
            foreach (var resource in resources)
            {
                ws.Cell(row, 1).Value = resource;
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0xE3F2FD);

                for (int c = 0; c < dates.Count; c++)
                {
                    var key = (resource, dates[c]);
                    var dataCell = ws.Cell(row, c + 2);
                    dataCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    dataCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    if (cells.TryGetValue(key, out var grid))
                    {
                        dataCell.Value = $"{grid.Hours:N1}h";

                        if (!string.IsNullOrEmpty(grid.Period))
                        {
                            dataCell.GetComment().AddText($"Período: {grid.Period}");
                            if (!string.IsNullOrEmpty(grid.WorkUnit))
                                dataCell.GetComment().AddNewLine().AddText($"Unidade: {grid.WorkUnit}");
                            dataCell.GetComment().AddNewLine().AddText($"Alocações: {grid.Count}");
                        }

                        // Intensity color (blue gradient based on hours)
                        var intensity = Math.Min(grid.Hours / 12.0, 1.0);
                        var r = (int)(255 - (255 - 33) * intensity);
                        var g = (int)(255 - (255 - 150) * intensity);
                        var b = (int)(255 - (255 - 243) * intensity);
                        dataCell.Style.Fill.BackgroundColor = XLColor.FromArgb(r, g, b);

                        if (intensity > 0.5)
                            dataCell.Style.Font.FontColor = XLColor.White;
                    }
                    else
                    {
                        dataCell.Style.Fill.BackgroundColor = XLColor.FromArgb(0xF5F5F5);
                    }

                    // Border
                    dataCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataCell.Style.Border.OutsideBorderColor = XLColor.FromArgb(0xE0E0E0);
                }
                row++;
            }

            // Summary row
            row++;
            ws.Cell(row, 1).Value = "Total por Dia";
            ws.Cell(row, 1).Style.Font.Bold = true;
            for (int c = 0; c < dates.Count; c++)
            {
                var dayTotal = cells
                    .Where(kvp => kvp.Key.Item2 == dates[c])
                    .Sum(kvp => kvp.Value.Hours);
                var totalCell = ws.Cell(row, c + 2);
                totalCell.Value = $"{dayTotal:N1}h";
                totalCell.Style.Font.Bold = true;
                totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                totalCell.Style.Fill.BackgroundColor = XLColor.FromArgb(0xE8EAF6);
            }

            // Formatting
            ws.Column(1).Width = 30;
            for (int c = 0; c < dates.Count; c++)
                ws.Column(c + 2).Width = 12;

            ws.SheetView.FreezeRows(4);
            ws.SheetView.FreezeColumns(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ── PDF grid export ─────────────────────────────────────────────

        public byte[] ExportScheduleGridToPdf(List<ScheduleAppointmentDto> appointments, string title, string subtitle)
        {
            var (resources, dates, cells) = BuildGrid(appointments);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    page.Header().Column(col =>
                    {
                        col.Item().Text(title).FontSize(18).Bold();
                        if (!string.IsNullOrEmpty(subtitle))
                            col.Item().Text(subtitle).FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingVertical(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingVertical(5).Table(table =>
                    {
                        // Column definitions: resource name + one per date
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(120); // resource name
                            foreach (var _ in dates)
                                cols.RelativeColumn();
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell()
                                .Background(Colors.Blue.Darken3)
                                .Padding(4)
                                .Text("Recurso").FontSize(7).Bold().FontColor(Colors.White);

                            foreach (var date in dates)
                            {
                                var isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
                                var bg = isWeekend ? Colors.BlueGrey.Darken3 : Colors.Blue.Darken3;

                                header.Cell()
                                    .Background(bg)
                                    .Padding(2)
                                    .AlignCenter()
                                    .Column(c =>
                                    {
                                        c.Item().Text($"{date:dd/MM}").FontSize(7).Bold().FontColor(Colors.White);
                                        c.Item().Text(DayNames[(int)date.DayOfWeek]).FontSize(6).FontColor(Colors.White);
                                    });
                            }
                        });

                        // Data rows
                        bool alternate = false;
                        foreach (var resource in resources)
                        {
                            var rowBg = alternate ? Colors.Grey.Lighten4 : Colors.White;

                            table.Cell()
                                .Background(rowBg)
                                .BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(3)
                                .Text(resource).FontSize(7).Bold();

                            foreach (var date in dates)
                            {
                                var key = (resource, date);
                                var cell = table.Cell()
                                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                    .Padding(2)
                                    .AlignCenter()
                                    .AlignMiddle();

                                if (cells.TryGetValue(key, out var grid))
                                {
                                    var intensity = Math.Min(grid.Hours / 12.0, 1.0);
                                    var r = (byte)(255 - (255 - 33) * intensity);
                                    var g = (byte)(255 - (255 - 150) * intensity);
                                    var b = (byte)(255 - (255 - 243) * intensity);

                                    cell.Background(Color.FromRGB(r, g, b))
                                        .Column(col =>
                                        {
                                            var fontColor = intensity > 0.5 ? Colors.White : Colors.Black;
                                            col.Item().AlignCenter().Text($"{grid.Hours:N1}h").FontSize(7).Bold().FontColor(fontColor);
                                            col.Item().AlignCenter().Text(grid.Period).FontSize(5).FontColor(fontColor);
                                        });
                                }
                                else
                                {
                                    cell.Background(Colors.Grey.Lighten4).Text("").FontSize(6);
                                }
                            }

                            alternate = !alternate;
                        }

                        // Summary row
                        table.Cell()
                            .Background(Colors.Indigo.Lighten5)
                            .Padding(3)
                            .Text("Total").FontSize(7).Bold();

                        foreach (var date in dates)
                        {
                            var dayTotal = cells
                                .Where(kvp => kvp.Key.Item2 == date)
                                .Sum(kvp => kvp.Value.Hours);

                            table.Cell()
                                .Background(Colors.Indigo.Lighten5)
                                .Padding(2)
                                .AlignCenter()
                                .Text($"{dayTotal:N1}h").FontSize(7).Bold();
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ").FontSize(8);
                        text.CurrentPageNumber().FontSize(8);
                        text.Span(" de ").FontSize(8);
                        text.TotalPages().FontSize(8);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
