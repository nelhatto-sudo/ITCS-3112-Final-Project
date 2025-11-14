namespace ITCS_3112_Final_Project;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new List<User>();

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public void RemoveUser(int userId)
    {
        _users.RemoveAll(x => x.UserId == userId);
    }

    public User GetUser(int userId)
    {
        return _users.FirstOrDefault(x => x.UserId == userId);

    }
    
}