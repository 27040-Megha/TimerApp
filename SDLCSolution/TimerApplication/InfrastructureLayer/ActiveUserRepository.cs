using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer
{
    public class ActiveUserRepository : IActiveUserRepository
    {
        public User FetchActiveUser()
        {
            if (!File.Exists(FilePathResource.LastActiveUser))
            {
                return null;
            }

            string existingJson = File.ReadAllText(FilePathResource.LastActiveUser);
            var user = JsonSerializer.Deserialize<User>(existingJson) ?? null;
            return user;
        }

        public void SaveCurrentActiveUser(User user)
        {
            string jsonTextToWrite = JsonSerializer.Serialize(user);
            File.WriteAllText(FilePathResource.LastActiveUser, jsonTextToWrite);
        }
    }
}
