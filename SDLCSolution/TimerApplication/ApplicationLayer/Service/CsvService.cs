using System.Globalization;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public static class CsvService
{
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
    private static readonly string[] ExpectedHeaders = { "Log Name", "Status", "Start Time", "End Time", "Duration" };

    public static string SerializeLogs(IEnumerable<ActivityLog> logsToExport)
    {
        if (logsToExport == null)
        {
            throw new ArgumentNullException(nameof(logsToExport), "Logs to export cannot be null.");
        }

        StringBuilder csvBuilder = new StringBuilder();
        csvBuilder.AppendLine(string.Join(", ", ExpectedHeaders));
        foreach (ActivityLog log in logsToExport)
        {
            string escapedName = EscapeCsvField(log.LogName!);
            string startTime = log.StartTime.ToString(DateFormat, CultureInfo.InvariantCulture);
            string endTime = log.EndTime!.Value.ToString(DateFormat, CultureInfo.InvariantCulture);
            string duration = log.Duration.ToString("c", CultureInfo.InvariantCulture);
            csvBuilder.AppendLine($"{escapedName}, {log.LogStatus}, {startTime}, {endTime}, {duration}");
        }
        return csvBuilder.ToString();
    }

    public static List<ActivityLog> DeserializeLogs(string csvContent, Guid taskId, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(csvContent))
        {
            throw new FormatException("The CSV file is empty or missing log entries.");
        }

        List<ActivityLog> logs = new List<ActivityLog>();
        using StringReader reader = new StringReader(csvContent);
        using TextFieldParser parser = new TextFieldParser(reader)
        {
            TextFieldType = FieldType.Delimited,
            Delimiters = new[] { "," },
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = true
        };

        if (parser.EndOfData)
        {
            throw new FormatException("The CSV file is empty or missing log entries.");
        }

        string[]? header = parser.ReadFields();
        if (header == null || !header.SequenceEqual(ExpectedHeaders, StringComparer.OrdinalIgnoreCase))
        {
            throw new FormatException("Invalid CSV format: Header does not match the expected format.");
        }

        while (!parser.EndOfData)
        {
            long lineNumber = parser.LineNumber;
            string[]? columns = parser.ReadFields();
            if (columns == null || columns.All(string.IsNullOrWhiteSpace)) continue;

            if (columns.Length != 5)
            {
                throw new FormatException($"Invalid CSV format at line {lineNumber}: Expected 5 columns.");
            }

            string logName = columns[0];
            if (string.IsNullOrWhiteSpace(logName))
            {
                throw new FormatException($"Invalid data format at line {lineNumber}: Log Name cannot be null or empty.");
            }

            if (!Enum.TryParse(columns[1], true, out TimerStatus logStatus) || logStatus != TimerStatus.Stopped)
            {
                throw new FormatException($"Invalid data format at line {lineNumber}: Status must be '{TimerStatus.Stopped}'.");
            }

            if (!DateTime.TryParseExact(columns[2], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime))
            {
                throw new FormatException($"Invalid data format at line {lineNumber}: '{columns[2]}' is invalid.");
            }

            if (!DateTime.TryParseExact(columns[3], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTime))
            {
                throw new FormatException($"Invalid data format at line {lineNumber}: '{columns[3]}' is invalid.");
            }

            if (!TimeSpan.TryParseExact(columns[4], "c", CultureInfo.InvariantCulture, out TimeSpan duration))
            {
                throw new FormatException($"Invalid data format at line {lineNumber}: '{columns[4]}' is invalid.");
            }

            if (endTime <= startTime)
            {
                throw new FormatException($"Invalid data format at line {lineNumber}: End Time must be after Start Time.");
            }

            TimeSpan computedDuration = endTime - startTime;
            if (Math.Abs((computedDuration - duration).TotalSeconds) > 1)
            {
                throw new FormatException($"Invalid data format at line {lineNumber}: Duration does not match Start and End times.");
            }

            logs.Add(new ActivityLog(Guid.NewGuid(), logName, startTime, endTime, computedDuration, TimerStatus.Stopped, taskId, userId));
        }

        if (!logs.Any())
        {
            throw new FormatException("The CSV file is empty or missing log entries.");
        }

        return logs;
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
