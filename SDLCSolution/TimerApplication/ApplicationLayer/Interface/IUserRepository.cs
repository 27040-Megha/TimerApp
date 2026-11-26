using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IUserRepository
{
    public void CreateUser(User user);

    public IReadOnlySet<User> FetchAllUsers();

    public User? FindByEmail(string? email);

    public User? FindById(Guid? userId);
}
