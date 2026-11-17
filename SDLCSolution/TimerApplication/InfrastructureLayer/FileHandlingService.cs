using System;
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

        public static List<ActivityLog> DeserializeActivityLogsFromCsv(string filePath, Guid taskId)
        {
            var logs = new List<ActivityLog>();
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The specified csv file is not present in {filePath}");
                }

                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length < 2)
                {
                    throw new FormatException("The CSV file is empty or missing log entries");
                }

                string expectedHeader = "Log Name, Status, Start Time, End Time, Duration";
                if (!lines[0].Trim().Equals(expectedHeader, StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException("Invalid CSV format: Header does not match the expected format");
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string[] columns = line.Split(',');
                    if (columns.Length != 6)
                    {
                        throw new FormatException($"Invalid CSV format at line {i+1}!");
                    }

                    string logName = columns[0].Trim();
                    if (string.IsNullOrWhiteSpace(logName))
                    {
                        throw new FormatException($"Invalid data format at line {i + 1}, {logName} cannot be null or empty");
                    }

                    if (!Enum.TryParse(columns[2].Trim(), true, out TimerStatus logStatus))
                    {
                        throw new FormatException($"Invalid data format at line {i + 1}, {columns[2]} is invalid");
                    }

                    if (!DateTime.TryParse(columns[3].Trim(), out DateTime startTime))
                    {
                        throw new FormatException($"Invalid data format at line {i + 1}, {columns[3]} is invalid");
                    }

                    if (!DateTime.TryParse(columns[4].Trim(), out DateTime endTime))
                    {
                        throw new FormatException($"Invalid data format at line {i + 1}, {columns[4]} is invalid");
                    }

                    if (!TimeSpan.TryParse(columns[5].Trim(), out TimeSpan duration))
                    {
                        throw new FormatException($"Invalid data format at line {i+1}, {columns[5]} is invalid");
                    }

                    var log = new ActivityLog(Guid.NewGuid(), logName, startTime, endTime, duration, logStatus, taskId, CurrentSessionManager.CurrentSessionUser.UserId);
                    logs.Add(log);
                }
                return logs;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to Deserialize CSV file, {ex.Message}");
            }
        }
    }
}
