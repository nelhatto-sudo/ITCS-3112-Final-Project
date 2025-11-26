namespace ITCS_3112_Final_Project;

public interface IUserRepository
{
    User? GetById(int userId);
    User? GetByUserName(string userName);
    IReadOnlyList<User> GetAll();
    User CreateUser(string userName);
    bool DeleteUser(int userId);
}