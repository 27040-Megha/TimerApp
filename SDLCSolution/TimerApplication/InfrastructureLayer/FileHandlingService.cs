using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TimerApplication.InfrastructureLayer
{
    public static class FileHandlingService
    {
        public static List<T> ReadFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            string existingJson = File.ReadAllText(filePath);
            var list = JsonSerializer.Deserialize<List<T>>(existingJson) ?? new List<T>();
            return list;
        }

        public static void WriteFile<T>(List<T> financialRecords, string filePath)
        {
            string jsonTextToWrite = JsonSerializer.Serialize(financialRecords);
            File.WriteAllText(filePath, jsonTextToWrite);
        }
    }
}
