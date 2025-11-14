namespace ITCS_3112_Final_Project;

public interface IUserRepository
{
    User? GetUser(int userId);
    void AddUser(User user);
    void RemoveUser(int userId);
}