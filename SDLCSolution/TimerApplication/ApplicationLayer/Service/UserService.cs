using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class UserService
{
    private readonly IUserRepository _userRepository;

    private readonly IActiveUserRepository _activeUserRepository;

    private readonly ISessionManager _currentSessionManager;

    public UserService(IUserRepository userRepository, IActiveUserRepository activeUserRepository, ISessionManager currentSessionManager)
    {
        this._userRepository = userRepository;
        this._activeUserRepository = activeUserRepository;
        this._currentSessionManager = currentSessionManager;
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

    private bool DoesEmailExists(string? email)
    {
        HashSet<string?> existingEmails = this.FetchAllUsers().Select(user => user.Email).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return existingEmails.Contains(email);
    }

    public IEnumerable<User> FetchAllUsers()
    {
        return this._userRepository.FetchAllUsers();
    }


    public AuthenticationStatus LoginUser(string? email, string? password)
    {
        User? matchingUser = this._userRepository.FindByEmail(email);
        if (matchingUser == null)
        {
            return AuthenticationStatus.AccountNotExists;
        }

        if (CheckPassword(password, matchingUser.Password))
        {
            this.SaveCurrentSessionUser(matchingUser);
            return AuthenticationStatus.Successful;
        }
        else
        {
            return AuthenticationStatus.InvalidPassword;
        }
    }

    public void SaveCurrentSessionUser(User? user)
    {
        _currentSessionManager.CurrentSessionUser = user;
        SaveCurrentActiveUser();
    }

    private bool CheckPassword(string? password, string? hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public void SaveCurrentActiveUser()
    {
        this._activeUserRepository.SaveCurrentActiveUser(_currentSessionManager.CurrentSessionUser?.UserId);
    }

    public Guid? FetchActiveUser()
    {
        return this._activeUserRepository.FetchActiveUser();
    }

    public User? FindUserById(Guid? userId)
    {
        return this._userRepository.FindById(userId);
    }

    public void ClearSession()
    {
        _currentSessionManager.CurrentSessionUser = null;
        this.SaveCurrentActiveUser();
    }
}

