using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer;

public static class FileHandlingService
{
    public static T ReadFile<T>(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return default;
            }

            string existingJson = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(existingJson))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(existingJson);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Data in file {filePath} is corrupted and can't be parsed.", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Unable to read from file {filePath}, the file may be locked or inaccessible.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied to read file {filePath}.", ex);
        }
    }

    public static void WriteFile<T>(T data, string filePath)
    {
        try
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonTextToWrite = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, jsonTextToWrite);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Data in file {filePath} is corrupted and can't be parsed.", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Unable to write to file {filePath}, the file may be locked or inaccessible.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied to write file {filePath}.", ex);
        }
    }

    private static string SerializeCsvContent(List<ActivityLog> logsToExport)
    {
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Log Name, Status, Start Time, End Time, Duration");
        foreach (var log in logsToExport)
        {
            var startTime = log.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            var endTime = log.EndTime.HasValue ? log.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : TimerStatus.Running.ToString();
            var duration = log.Duration.ToString(@"hh\:mm\:ss");
            csvBuilder.AppendLine($"{log.LogName}, {log.LogStatus}, {startTime}, {endTime}, {duration}");
        }

        return csvBuilder.ToString();
    }

    public static List<ActivityLog> DeserializeActivityLogsFromCsv(string filePath, Guid taskId)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified CSV file is not present in {filePath}");
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            ValidateHeader(lines);

            var logs = new List<ActivityLog>();
            Guid userId = CurrentSessionManager.CurrentSessionUser.UserId;

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                logs.Add(ParseLogLine(lines[i], i + 1, taskId, userId));
            }

            return logs;
        }
        catch (FormatException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FormatException($"Failed to deserialize CSV file: {ex.Message}", ex);
        }
    }

    private static void ValidateHeader(string[] lines)
    {
        if (lines.Length < 2)
        {
            throw new FormatException("The CSV file is empty or missing log entries.");
        }

        const string expectedHeader = "Log Name, Status, Start Time, End Time, Duration";
        if (!lines[0].Trim().Equals(expectedHeader, StringComparison.OrdinalIgnoreCase))
        {
            throw new FormatException("Invalid CSV format: Header does not match the expected format.");
        }
    }

    private static ActivityLog ParseLogLine(string line, int lineNumber, Guid taskId, Guid userId)
    {
        string[] columns = line.Split(',');
        if (columns.Length != 5)
        {
            throw new FormatException($"Invalid CSV format at line {lineNumber}!");
        }

        string logName = columns[0].Trim();
        if (string.IsNullOrWhiteSpace(logName))
        {
            throw new FormatException($"Invalid data format at line {lineNumber}: Log Name cannot be null or empty.");
        }

        if (!Enum.TryParse(columns[1].Trim(), true, out TimerStatus logStatus))
        {
            throw new FormatException($"Invalid data format at line {lineNumber}: '{columns[1].Trim()}' is invalid.");
        }

        if (!DateTime.TryParse(columns[2].Trim(), out DateTime startTime))
        {
            throw new FormatException($"Invalid data format at line {lineNumber}: '{columns[2].Trim()}' is invalid.");
        }

        if (!DateTime.TryParse(columns[3].Trim(), out DateTime endTime))
        {
            throw new FormatException($"Invalid data format at line {lineNumber}: '{columns[3].Trim()}' is invalid.");
        }

        if (!TimeSpan.TryParse(columns[4].Trim(), out TimeSpan duration))
        {
            throw new FormatException($"Invalid data format at line {lineNumber}: '{columns[4].Trim()}' is invalid.");
        }

        return new ActivityLog(Guid.NewGuid(), logName, startTime, endTime, duration, logStatus, taskId, userId);
    }


    public static void ExportToCSV(string fileName, List<ActivityLog> logsToExport)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(fileName));
        }

        if (logsToExport == null)
        {
            throw new ArgumentNullException(nameof(logsToExport), "Logs to export cannot be null.");
        }

        try
        {
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string targetFolder = Directory.Exists(downloadsFolder)
                ? downloadsFolder
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(targetFolder, fileName);
            string csvContent = SerializeCsvContent(logsToExport);
            File.WriteAllText(fullPath, csvContent);
        }
        catch (IOException ex)
        {
            throw new IOException($"Unable to write to CSV file {fileName}, the file may be locked or inaccessible.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied to write CSV file {fileName}.", ex);
        }
    }

}
