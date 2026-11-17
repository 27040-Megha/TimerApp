using System;
using System.IO;
using System.Text;
using System.Text.Json;
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

        private static string SerializeCsvContent(List<ActivityLog> logsToExport)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Log Name, Status, Start Time, End Time, Duration");
            foreach (var log in logsToExport)
            {
                var startTime = log.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = log.EndTime.HasValue ? log.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "In-Progress";
                var duration = log.Duration.ToString(@"hh\:mm\:ss");
                csvBuilder.AppendLine($"{log.LogName}, {log.LogStatus}, {startTime}, {endTime}, {duration}");
            }

            return csvBuilder.ToString();
        }
    }
}
