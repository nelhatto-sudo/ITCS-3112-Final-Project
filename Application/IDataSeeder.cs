namespace ITCS_3112_Final_Project;

public interface IDataSeeder
{
    void Seed(ICatalog catalog, IUserRepository userRepository, IGameRepository gameRepository);
}