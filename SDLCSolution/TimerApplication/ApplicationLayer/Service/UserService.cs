using System;
using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    public bool CreateUser(User newUser)
    {
        if (DoesEmailExists(newUser.Email))
        {
            return false;
        }

        this._userRepository.CreateUser(newUser);
        return true;
    }

    private bool DoesEmailExists(string email)
    {
        var existingEmails = this.FetchAllUsers().Select(user => user.Email).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return existingEmails.Contains(email);
    }

    public IEnumerable<User> FetchAllUsers()
    {
        return this._userRepository.FetchAllUsers();
    }
}
