using System.Text;
using System.Text.Json;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

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
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string tempFilePath = Path.Combine(
            directory ?? string.Empty,
            $"{Path.GetFileName(filePath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            string jsonTextToWrite = JsonSerializer.Serialize(data);
            File.WriteAllText(tempFilePath, jsonTextToWrite);
            File.Move(tempFilePath, filePath, overwrite: true);
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
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    public static string ReadTextFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified file was not found at {filePath}");
        }

        try
        {
            return File.ReadAllText(filePath);
        }
        catch (IOException ex)
        {
            throw new IOException($"Unable to read file {filePath}, the file may be locked or inaccessible.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied to read file {filePath}.", ex);
        }
    }

    public static string WriteCsvToDownloads(string rawFileName, string csvContent)
    {
        if (string.IsNullOrWhiteSpace(rawFileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(rawFileName));
        }

        string safeFileName = SanitizeFileName(rawFileName);
        try
        {
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string targetFolder = Directory.Exists(downloadsFolder)
                ? downloadsFolder
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fullPath = Path.Combine(targetFolder, safeFileName);
            File.WriteAllText(fullPath, csvContent);
            return fullPath;
        }
        catch (IOException ex)
        {
            throw new IOException($"Unable to write to CSV file {safeFileName}, the file may be locked or inaccessible.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied to write CSV file {safeFileName}.", ex);
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string cleaned = string.Concat(fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "export.csv" : cleaned;
    }
}
