namespace ITCS_3112_Final_Project;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _users = new();
    private int _nextUserId = 1;

    public User? GetById(int userId)
    {
        _users.TryGetValue(userId, out var user);
        return user;
    }

    public User? GetByUserName(string userName)
    {
        foreach (var user in _users.Values)
        {
            if (string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase))
                return user;
        }
        return null;
    }

    public IReadOnlyList<User> GetAll()
    {
        return _users.Values.ToList().AsReadOnly();
    }

    public User CreateUser(string userName)
    {
        var id = _nextUserId++;
        var user = new User(id, userName);
        _users.Add(id, user);
        return user;
    }

    public bool DeleteUser(int userId)
    {
        return _users.Remove(userId);
    }
}