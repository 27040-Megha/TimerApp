using System;
using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class UserService
{
    private readonly IUserRepository _userRepository;

    private readonly IActiveUserRepository _activeUserRepository;

    public UserService(IUserRepository userRepository, IActiveUserRepository activeUserRepository)
    {
        this._userRepository = userRepository;
        this._activeUserRepository = activeUserRepository;
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


    public Result LoginUser(string email, string password)
    {
        var users = this.FetchAllUsers();
        var matchingUser = users.FirstOrDefault(u => string.Equals(u.Email, email));
        if (matchingUser == null)
        {
            return new Result(false, AuthenticationStatus.AccountNotExists);
        }

        if (CheckPassword(password, matchingUser.Password))
        {
            CurrentSessionManager.CurrentSessionUser = matchingUser;
            return new Result(true, AuthenticationStatus.Successful);
        }
        else
        {
            return new Result(false, AuthenticationStatus.InvalidPassword);
        }
    }

    private bool CheckPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public void SaveCurrentActiveUser()
    {
        this._activeUserRepository.SaveCurrentActiveUser(CurrentSessionManager.CurrentSessionUser);
    }

    public User FetchActiveUser()
    {
        return this._activeUserRepository.FetchActiveUser();
    }
}

