using System.Text.Json;

namespace TimerApplication.InfrastructureLayer;

public static class FileHandlingService
{
    public static T? ReadFile<T>(string filePath)
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
            string? directory = Path.GetDirectoryName(filePath);
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
}
